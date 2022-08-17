using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Common;

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
            MainDb.CurrentDbConnId = Appsettings.App(new string[] { "MainDB" });

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
                            OnLogExecuting = (sql, p) =>
                            {
                                if (Appsettings.App(new string[] { "AppSettings", "SqlAOP", "OutToLogFile", "Enabled" }).ToBoolReq())
                                {
                                    Parallel.For(0, 1, e =>
                                    {
                                        MiniProfiler.Current.CustomTiming("SQL：", GetParas(p) + "【SQL语句】：" + sql);
                                        // LogLock.OutSql2Log("SqlLog", new string[] { GetParas(p), "【SQL语句】：" + sql });
                                        SerilogServer.WriteLog("SqlLog", new string[] { GetParas(p), "【SQL语句】：" + sql });
                                    });
                                }
                                if (Appsettings.App(new string[] { "AppSettings", "SqlAOP", "OutToConsole", "Enabled" }).ToBoolReq())
                                {
                                    ConsoleHelper.WriteColorLine(string.Join("\r\n", new string[] { "--------", "【SQL语句】：" + GetWholeSql(p, sql) }), ConsoleColor.DarkCyan);
                                }
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
                 .Add(new SqlFilterItem()//单表全局过滤器
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