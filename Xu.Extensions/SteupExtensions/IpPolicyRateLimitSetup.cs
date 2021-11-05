using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Xu.Extensions
{
    /// <summary>
    /// IPLimit限流 启动服务
    /// </summary>
    public static class IpPolicyRateLimitSetup
    {
        /// <summary>
        ///  IPLimit限流
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static void AddIpPolicyRateLimitSetup(this IServiceCollection services, IConfiguration Configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //需要存储速率限制计算器和ip规则
            services.AddMemoryCache();

            //从appsettings.json获取相应配置
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //注入计数器和规则存储
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // inject counter and rules distributed cache stores
            //services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            //配置（计数器密钥生成器）
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}