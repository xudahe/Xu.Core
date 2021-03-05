using Microsoft.Extensions.DependencyInjection;
using System;
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
    }
}