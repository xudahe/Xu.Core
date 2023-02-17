using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using Xu.Common;
using Xu.EventBus;

namespace Xu.Extensions
{
    /// <summary>
    /// Db 启动服务
    /// </summary>
    public static class RabbitMQSetup
    {
        public static void AddRabbitMQSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (AppSettings.App(new string[] { "RabbitMQ", "Enabled" }).ToBoolReq())
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                   {
                       var logger = sp.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();

                       var factory = new ConnectionFactory()
                       {
                           HostName = AppSettings.App(new string[] { "RabbitMQ", "Connection" }),
                           DispatchConsumersAsync = true
                       };

                       if (!string.IsNullOrEmpty(AppSettings.App(new string[] { "RabbitMQ", "UserName" })))
                       {
                           factory.UserName = AppSettings.App(new string[] { "RabbitMQ", "UserName" });
                       }

                       if (!string.IsNullOrEmpty(AppSettings.App(new string[] { "RabbitMQ", "Password" })))
                       {
                           factory.Password = AppSettings.App(new string[] { "RabbitMQ", "Password" });
                       }

                       if (!string.IsNullOrEmpty(AppSettings.App(new string[] { "RabbitMQ", "Port" })))
                       {
                           factory.Port = AppSettings.App(new string[] { "RabbitMQ", "Port" }).ToInt32Req();
                       }

                       var retryCount = 5;
                       if (!string.IsNullOrEmpty(AppSettings.App(new string[] { "RabbitMQ", "RetryCount" })))
                       {
                           retryCount = AppSettings.App(new string[] { "RabbitMQ", "RetryCount" }).ToInt32Req();
                       }

                       return new RabbitMQPersistentConnection(factory, logger, retryCount);
                   });
            }
        }
    }
}