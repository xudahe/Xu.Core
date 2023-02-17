using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xu.Common;
using Xu.EventBus;

namespace Xu.Extensions
{
    /// <summary>
    /// Kafka 消息队列
    /// </summary>
    public static class KafkaSetup
    {
        public static void AddKafkaSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (AppSettings.App(new string[] { "Kafka", "Enabled" }).ToBoolReq())
            {
                services.Configure<KafkaOptions>(configuration.GetSection("kafka"));
                services.AddSingleton<IKafkaConnectionPool, KafkaConnectionPool>();
            }
        }
    }
}