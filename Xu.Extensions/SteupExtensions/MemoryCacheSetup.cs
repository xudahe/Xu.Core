using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddScoped<IMemoryCaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                return new MemoryCache(new MemoryCacheOptions());
            });
        }
    }
}