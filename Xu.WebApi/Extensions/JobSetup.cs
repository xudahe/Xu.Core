using Microsoft.Extensions.DependencyInjection;
using System;
using Xu.Tasks;

namespace Xu.WebApi
{
    /// <summary>
    /// IHostedService  启动服务
    /// </summary>
    public static class JobSetup
    {
        /// <summary>
        /// 实现定时任务
        /// </summary>
        /// <param name="services"></param>
        public static void AddJobSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHostedService<Job1Timed>();
            services.AddHostedService<Job2Timed>();
        }
    }
}