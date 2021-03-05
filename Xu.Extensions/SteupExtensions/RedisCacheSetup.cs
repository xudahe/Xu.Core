using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// Redis缓存 启动服务
    /// </summary>
    public static class RedisCacheSetup
    {
        /// <summary>
        /// Redis缓存
        /// </summary>
        /// <param name="services"></param>
        public static void AddRedisCacheSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IRedisCache, RedisCache>();

            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                //获取连接字符串
                string redisConfiguration = Appsettings.App(new string[] { "Redis", "ConnectionString" });

                var configuration = ConfigurationOptions.Parse(redisConfiguration, true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });
        }
    }
}