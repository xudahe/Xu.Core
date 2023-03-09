using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Common;
using Xu.Common.MemoryCache;
using Xu.Model;

namespace Xu.Extensions
{
    /// <summary>
    /// SqlSugar 启动服务
    /// </summary>
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // 默认添加主数据库连接
            MainDb.CurrentDbConnId = AppSettings.App(new string[] { "MainDB" });

            // SqlSugarScope是线程安全，可使用单例注入
            // 参考：https://www.donet5.com/Home/Doc?typeId=1181
            services.AddScoped<ISqlSugarClient>(o =>
            {
                var memoryCache = o.GetRequiredService<IMemoryCache>();

                // 连接字符串
                var listConfig = new List<ConnectionConfig>();
                // 从库
                var listConfig_Slave = new List<SlaveConnectionConfig>();
                BaseDBConfig.MutiConnectionString.slaveDbs.ForEach(s =>
                {
                    listConfig_Slave.Add(new SlaveConnectionConfig()
                    {
                        HitRate = s.HitRate,
                        ConnectionString = s.Connection
                    });
                });

                BaseDBConfig.MutiConnectionString.allDbs.ForEach(m =>
                {
                    listConfig.Add(new ConnectionConfig()
                    {
                        ConfigId = m.ConnId.ObjToString().ToLower(), //设置库的唯一标识
                        ConnectionString = m.Connection, //必填, 数据库连接字符串
                        DbType = (DbType)m.DbType, //必填, 数据库类型
                        IsAutoCloseConnection = true, //是否关闭数据库连接, 设置为true无需使用using或者Close操作
                        //IsShardSameThread = false, //默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
                        AopEvents = new AopEvents
                        {
                            //Sql执行前事件
                            OnLogExecuting = (sql, p) =>
                            {
                                if (AppSettings.App(new string[] { "AppSettings", "SqlAOP", "LogToFile", "Enabled" }).ToBoolReq())
                                {
                                    Parallel.For(0, 1, e =>
                                    {
                                        MiniProfiler.Current.CustomTiming("SQL：", GetParas(p) + "【SQL语句】：" + sql);
                                        LogLock.OutLogAOP("SqlLog", "", new string[] { sql.GetType().ToString(), GetParas(p), "【SQL语句】：" + sql });
                                    });
                                    //SerilogServer.WriteLog("SqlLog", new string[] { GetParas(p), "【SQL语句】：" + sql });
                                }
                                if (AppSettings.App(new string[] { "AppSettings", "SqlAOP", "LogToConsole", "Enabled" }).ToBoolReq())
                                {
                                    ConsoleHelper.WriteColorLine(string.Join("\r\n", new string[] { "--------", "【SQL语句】：" + GetWholeSql(p, sql) }), ConsoleColor.DarkCyan);
                                }
                            },
                            //插入或者更新可以修改 实体里面的值，比如插入或者更新 赋默认值
                            DataExecuting = (oldValue, entityInfo) =>
                            {
                                if (entityInfo.EntityValue is ModelBase baseEntity)
                                {
                                    /*** inset生效 ***/
                                    if (entityInfo.OperationType == DataFilterType.InsertByObject)
                                    {
                                        baseEntity.CreateTime = DateTime.Now;
                                        baseEntity.Guid = !baseEntity.Guid.IsNotEmptyOrNull() ? GUIDHelper.Guid32() : baseEntity.Guid;
                                    }

                                    /*** update生效 ***/
                                    if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                                    {
                                        baseEntity.ModifyTime = DateTime.Now;
                                    }
                                }
                            },
                        },
                        MoreSettings = new ConnMoreSettings()
                        {
                            IsAutoRemoveDataCache = true,
                            DbMinDate = DateTime.MinValue//默认最小时间是 1900-01-01 00:00:00.000 
                        },
                        // 从库
                        SlaveConnectionConfigs = listConfig_Slave,
                        // 自定义特性
                        ConfigureExternalServices = new ConfigureExternalServices()
                        {
                            DataInfoCacheService = new SqlSugarMemoryCacheService(memoryCache),
                            EntityService = (property, column) =>
                            {
                                //如果是主键，并且是int，才会增加它的自增属性，否则不处理。
                                if (column.IsPrimarykey && property.PropertyType == typeof(int))
                                {
                                    column.IsIdentity = true;
                                }
                            }
                        },
                        InitKeyType = InitKeyType.Attribute
                    }
                   );
                });
                SqlSugarScope db = new SqlSugarScope(listConfig);

                db.QueryFilter
                // 依赖实体过滤器， 配置实体软删除过滤器
                // 统一过滤 软删除 无需自己写条件
                // 依赖实体过滤器，ModelBase是自定义接口,继承这个接口的实体有效
                .AddTableFilter<ModelBase>(it => it.DeleteTime == null)
                //不依赖实体过滤器
                .Add(new SqlFilterItem()
                {
                    FilterValue = filterDb =>
                    {
                        return new SqlFilterResult() { Sql = " DeleteTime IS NULL" };
                    },
                    IsJoinQuery = false  //单表查询生效
                });

                return db;
            });
        }

        private static string GetWholeSql(SugarParameter[] paramArr, string sql)
        {
            foreach (var param in paramArr)
            {
                sql.Replace(param.ParameterName, param.Value.ObjToString());
            }

            return sql;
        }

        private static string GetParas(SugarParameter[] pars)
        {
            string key = "【SQL参数】：";
            foreach (var param in pars)
            {
                key += $"{param.ParameterName}:{param.Value}\n";
            }

            return key;
        }
    }
}