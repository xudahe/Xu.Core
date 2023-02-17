using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Profiling;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Common;
using Xu.Model;
using Xu.Model.Models;

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

            // 把多个连接对象注入服务，这里必须采用Scope，因为有事务操作
            services.AddScoped<ISqlSugarClient>(o =>
            {
                // 连接字符串
                var listConfig = new List<ConnectionConfig>();
                // 从库
                var listConfig_Slave = new List<SlaveConnectionConfig>();
                BaseDBConfig.MutiConnectionString.Item2.ForEach(s =>
                {
                    listConfig_Slave.Add(new SlaveConnectionConfig()
                    {
                        HitRate = s.HitRate,
                        ConnectionString = s.Connection
                    });
                });

                BaseDBConfig.MutiConnectionString.Item1.ForEach(m =>
                {
                    listConfig.Add(new ConnectionConfig()
                    {
                        ConfigId = m.ConnId.ToString().ToLower(), //设置库的唯一标识
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
                            //SQL执行后事件
                            OnLogExecuted = (sql, p) =>
                            {
                             
                            },
                            //SQL报错
                            OnError = (exp) =>
                            {
                                 
                            },
                            //可以修改SQL和参数的值
                            OnExecutingChangeSql = (sql, pars) =>
                            {
                                //sql=newsql
                                //foreach(var p in pars) //修改
                                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
                            },
                            //插入或者更新可以修改 实体里面的值，比如插入或者更新 赋默认值
                            DataExecuting = (oldValue, entityInfo) =>
                            {
                                /*** inset生效 ***/
                                if (entityInfo.PropertyName == "CreateTime" && entityInfo.OperationType == DataFilterType.InsertByObject)
                                {
                                    entityInfo.SetValue(DateTime.Now);//修改CreateTime字段
                                }

                                /*** update生效 ***/
                                if (entityInfo.PropertyName == "ModifyTime" && entityInfo.OperationType == DataFilterType.UpdateByObject)
                                {
                                    entityInfo.SetValue(DateTime.Now);//修改ModifyTime字段
                                }
                            },
                            DataExecuted = (value, entity) =>
                            {
                                //查询数据转换 User实体名: 查询出来的值的 name都加上了 111
                                //if (entity.Entity.Type == typeof(User))
                                //{
                                //    var newValue = entity.GetValue(nameof(User.RealName)) + "111";
                                //    entity.SetValue(nameof(User.RealName), newValue);
                                //}

                                //DataExecuting 和 DataExecuted一起用就可以实现加密和解密字段
                            }
                        },
                        MoreSettings = new ConnMoreSettings()
                        {
                            IsAutoRemoveDataCache = true
                        },
                        // 从库
                        SlaveConnectionConfigs = listConfig_Slave,
                        // 自定义特性
                        ConfigureExternalServices = new ConfigureExternalServices()
                        {
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
                //依赖实体过滤器
                .AddTableFilter<ModelBase>(it => it.DeleteTime == null)//ModelBase是自定义接口,继承这个接口的实体有效
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