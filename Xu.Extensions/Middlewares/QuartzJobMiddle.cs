using log4net;
using Microsoft.AspNetCore.Builder;
using System;
using Xu.Common;
using Xu.EnumHelper;
using Xu.IServices;
using Xu.Tasks;

namespace Xu.Extensions.Middlewares
{
    /// <summary>
    /// Quartz 启动服务（定时任务）
    /// </summary>
    public static class QuartzJobMiddle
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(QuartzJobMiddle));

        public static void UseQuartzJobMiddle(this IApplicationBuilder app, ITasksQzSvc tasksQzSvc, ISchedulerCenter schedulerCenter)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppSettings.App("Middleware", "QuartzNetJob", "Enabled").ToBoolReq())
                {
                    var allQzServices = tasksQzSvc.Query().Result;
                    foreach (var item in allQzServices)
                    {
                        if (item.JobStatus == JobStatus.运行中)
                        {
                            var ResuleModel = schedulerCenter.AddScheduleJobAsync(item).Result;
                            if (ResuleModel.Success)
                            {
                                Console.WriteLine($"QuartzNetJob{item.JobName}启动成功！");
                            }
                            else
                            {
                                Console.WriteLine($"QuartzNetJob{item.JobName}启动失败！错误信息：{ResuleModel.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error($"An error was reported when starting the job service.\n{e.Message}");
                throw;
            }
        }
    }
}