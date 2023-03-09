using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xu.Common;
using Xu.EventBus;
using Xu.Extensions.EventHandling;

namespace Xu.Extensions
{
    /// <summary>
    /// EventBus 事件总线服务
    /// </summary>
    public static class EventBusSetup
    {
        public static void AddEventBusSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (AppSettings.App(new string[] { "EventBus", "Enabled" }).ToBoolReq())
            {
                var subscriptionClientName = AppSettings.App(new string[] { "EventBus", "SubscriptionClientName" });

                services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
                services.AddTransient<ProductPriceChangedIntegrationEventHandler>();

                if (AppSettings.App(new string[] { "RabbitMQ", "Enabled" }).ToBoolReq())
                {
                    services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                    {
                        var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                        var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                        var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                        var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                        var retryCount = 5;
                        if (!string.IsNullOrEmpty(AppSettings.App(new string[] { "RabbitMQ", "RetryCount" })))
                        {
                            retryCount = int.Parse(AppSettings.App(new string[] { "RabbitMQ", "RetryCount" }));
                        }

                        return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
                    });
                }
                if (AppSettings.App(new string[] { "Kafka", "Enabled" }).ToBoolReq())
                {
                    services.AddHostedService<KafkaConsumerHostService>();
                    services.AddSingleton<IEventBus, EventBusKafka>();
                }
            }
        }
    }
}