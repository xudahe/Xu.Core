using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace Xu.Tasks
{
    /// <summary>
    /// 任务调度工厂实现，为了调用作业的参数构造函数，Quartz.NET提供了IJobFactory接口
    /// </summary>
    public class JobFactory : IJobFactory
    {
        /// <summary>
        /// 注入反射获取依赖对象
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)); ;
        }

        /// <summary>
        /// 实现接口Job
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            //return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;

            try
            {
                var serviceScope = _serviceProvider.CreateScope();
                var job = serviceScope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
                return job;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 清理销毁
        /// </summary>
        /// <param name="job"></param>
        public void ReturnJob(IJob job)
        {
            if (job is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}