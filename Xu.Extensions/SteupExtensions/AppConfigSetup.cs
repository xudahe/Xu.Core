using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// 项目 启动服务
    /// </summary>
    public static class AppConfigSetup
    {
        public static void AddAppConfigSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (Appsettings.App(new string[] { "Startup", "AppConfigAlert", "Enabled" }).ToBoolReq())
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.OutputEncoding = Encoding.GetEncoding("GB2312"); //避免乱码

                Console.WriteLine("************ WebApi Config Set *****************");

                ConsoleHelper.WriteSuccessLine("Current environment: " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

                // 授权策略方案
                if (Permissions.IsUseIds4)
                {
                    ConsoleHelper.WriteSuccessLine($"Current authorization scheme: " + (Permissions.IsUseIds4 ? "Ids4" : "JWT"));
                }
                else
                {
                    Console.WriteLine($"Current authorization scheme: " + (Permissions.IsUseIds4 ? "Ids4" : "JWT"));
                }

                // Redis缓存AOP
                if (!Appsettings.App(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).ToBoolReq())
                {
                    Console.WriteLine($"Redis Caching AOP: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"Redis Caching AOP: True");
                }

                // 内存缓存AOP
                if (!Appsettings.App(new string[] { "AppSettings", "MemoryCachingAOP", "Enabled" }).ToBoolReq())
                {
                    Console.WriteLine($"Memory Caching AOP: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"Memory Caching AOP: True");
                }

                // 服务日志AOP
                if (!Appsettings.App(new string[] { "AppSettings", "LogAOP", "Enabled" }).ToBoolReq())
                {
                    Console.WriteLine($"Service Log AOP: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"Service Log AOP: True");
                }

                // 事务AOP
                if (!Appsettings.App(new string[] { "AppSettings", "TranAOP", "Enabled" }).ToBoolReq())
                {
                    Console.WriteLine($"Transaction AOP: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"Transaction AOP: True");
                }

                // 数据库Sql执行AOP
                if (!Appsettings.App(new string[] { "AppSettings", "SqlAOP", "Enabled" }).ToBoolReq())
                {
                    Console.WriteLine($"DB Sql AOP: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"DB Sql AOP: True");
                }

                // SingnalR发送数据
                if (!Appsettings.App(new string[] { "Middleware", "SignalR", "Enabled" }).ToBoolReq())
                {
                    Console.WriteLine($"SignalR send data: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"SignalR send data: True");
                }

                // IP限流
                if (!Appsettings.App("Middleware", "IpRateLimit", "Enabled").ToBoolReq())
                {
                    Console.WriteLine($"IpRateLimiting: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"IpRateLimiting: True");
                }

                // 性能分析
                if (!Appsettings.App("Startup", "MiniProfiler", "Enabled").ToBoolReq())
                {
                    Console.WriteLine($"MiniProfiler: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"MiniProfiler: True");
                }

                // CORS跨域
                if (!Appsettings.App("Startup", "Cors", "EnableAllIPs").ToBoolReq())
                {
                    Console.WriteLine($"EnableAllIPs For CORS: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"EnableAllIPs For CORS: True");
                }

                // redis消息队列
                if (!Appsettings.App("Startup", "RedisMq", "Enabled").ToBoolReq())
                {
                    Console.WriteLine($"Redis MQ: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"Redis MQ: True");
                }

                // Consul 注册服务
                if (!Appsettings.App("Middleware", "Consul", "Enabled").ToBoolReq())
                {
                    Console.WriteLine($"Consul service: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"Consul service: True");
                }

                // 多库
                if (!Appsettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq())
                {
                    Console.WriteLine($"Is multi-DataBase: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"Is multi-DataBase: True");
                }

                // 读写分离
                if (!Appsettings.App(new string[] { "CQRSEnabled" }).ToBoolReq())
                {
                    Console.WriteLine($"Is CQRS: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine($"Is CQRS: True");
                }

                Console.WriteLine();
            }
        }

        public static void AddAppTableConfigSetup(this IServiceCollection services, IHostEnvironment env)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (Appsettings.App(new string[] { "Startup", "AppConfigAlert", "Enabled" }).ToBoolReq())
            {
                if (env.IsDevelopment())
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    Console.OutputEncoding = Encoding.GetEncoding("GB2312");
                }

                #region 程序配置

                List<string[]> configInfos = new()
                {
                    new string[] { "当前环境", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") },
                    new string[] { "当前的授权方案", Permissions.IsUseIds4 ? "Ids4" : "JWT" },
                    new string[] { "CORS跨域", Appsettings.App("Startup", "Cors", "EnableAllIPs") },
                    new string[] { "RabbitMQ消息列队", Appsettings.App("RabbitMQ", "Enabled") },
                    new string[] { "事件总线(必须开启消息列队)", Appsettings.App("EventBus", "Enabled") },
                    new string[] { "redis消息队列", Appsettings.App("Startup", "RedisMq", "Enabled") },
                    new string[] { "是否多库", Appsettings.App("MutiDBEnabled") },
                    new string[] { "读写分离", Appsettings.App("CQRSEnabled") },
                };

                new ConsoleTable()
                {
                    TitleString = "Blog.Core 配置集",
                    Columns = new string[] { "配置名称", "配置信息/是否启动" },
                    Rows = configInfos,
                    EnableCount = false,
                    Alignment = Alignment.Left,
                    ColumnBlankNum = 4,
                    TableStyle = TableStyle.Alternative
                }.Writer(ConsoleColor.Blue);
                Console.WriteLine();

                #endregion 程序配置

                #region AOP

                List<string[]> aopInfos = new()
                {
                    new string[] { "Redis缓存AOP", Appsettings.App("AppSettings", "RedisCachingAOP", "Enabled") },
                    new string[] { "内存缓存AOP", Appsettings.App("AppSettings", "MemoryCachingAOP", "Enabled") },
                    new string[] { "服务日志AOP", Appsettings.App("AppSettings", "LogAOP", "Enabled") },
                    new string[] { "事务AOP", Appsettings.App("AppSettings", "TranAOP", "Enabled") },
                    new string[] { "Sql执行AOP", Appsettings.App("AppSettings", "SqlAOP", "OutToLogFile", "Enabled") },
                    new string[] { "Sql执行AOP控制台输出", Appsettings.App("AppSettings", "SqlAOP", "OutToConsole", "Enabled") },
                };

                new ConsoleTable
                {
                    TitleString = "AOP",
                    Columns = new string[] { "配置名称", "配置信息/是否启动" },
                    Rows = aopInfos,
                    EnableCount = false,
                    Alignment = Alignment.Left,
                    ColumnBlankNum = 7,
                    TableStyle = TableStyle.Alternative
                }.Writer(ConsoleColor.Blue);
                Console.WriteLine();

                #endregion AOP

                #region 中间件

                List<string[]> MiddlewareInfos = new()
                {
                    new string[] { "请求纪录中间件", Appsettings.App("Middleware", "RecordAccessLogs", "Enabled") },
                    new string[] { "IP记录中间件", Appsettings.App("Middleware", "IPLog", "Enabled") },
                    new string[] { "请求响应日志中间件", Appsettings.App("Middleware", "RequestResponseLog", "Enabled") },
                    new string[] { "SingnalR实时发送请求数据中间件", Appsettings.App("Middleware", "SignalR", "Enabled") },
                    new string[] { "IP限流中间件", Appsettings.App("Middleware", "IpRateLimit", "Enabled") },
                    new string[] { "性能分析中间件", Appsettings.App("Startup", "MiniProfiler", "Enabled") },
                    new string[] { "Consul注册服务", Appsettings.App("Middleware", "Consul", "Enabled") },
                };

                new ConsoleTable
                {
                    TitleString = "中间件",
                    Columns = new string[] { "配置名称", "配置信息/是否启动" },
                    Rows = MiddlewareInfos,
                    EnableCount = false,
                    Alignment = Alignment.Left,
                    ColumnBlankNum = 3,
                    TableStyle = TableStyle.Alternative
                }.Writer(ConsoleColor.Blue);
                Console.WriteLine();

                #endregion 中间件
            }
        }
    }
}