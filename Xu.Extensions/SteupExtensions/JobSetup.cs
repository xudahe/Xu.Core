using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using System;
using Xu.Tasks;

namespace Xu.Extensions
{
    /// <summary>
    /// 任务调度 启动服务
    /// </summary>
    public static class JobSetup
    {
        public static void AddJobSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //services.AddHostedService<Job1TimedService>();
            //services.AddHostedService<Job2TimedService>();

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddTransient<JobQuartz>(); //这里使用瞬时依赖注入
            services.AddSingleton<ISchedulerCenter, SchedulerCenter>();
        }
    }
}