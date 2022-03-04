using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// Memory缓存 启动服务
    /// </summary>
    public static class MemoryCacheSetup
    {
        /// <summary>
        /// Memory缓存
        /// </summary>
        /// <param name="services"></param>
        public static void AddMemoryCacheSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<ICaching, CachingProvider>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var value = factory.GetRequiredService<IOptions<MemoryCacheOptions>>();
                var cache = new MemoryCache(value);
                return cache;
            });
        }
    }
}