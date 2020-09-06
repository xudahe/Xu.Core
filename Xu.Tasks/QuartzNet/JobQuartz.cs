using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;

/// <summary>
/// 这里要注意下，命名空间和程序集是一样的，不然反射不到
/// </summary>
namespace Xu.Tasks
{
    public class JobQuartz : IJob
    {
        private readonly ITasksQzSvc _tasksQzSvc;

        public JobQuartz(ITasksQzSvc tasksQzSvc)
        {
            _tasksQzSvc = tasksQzSvc;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await ExecuteJob(context, async () => await Run(context, context.JobDetail.Key.Name.ToInt32Req()));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            if (jobid > 0)
            {
                var separator = "<br>";
                var model = await _tasksQzSvc.QueryById(jobid);
                if (model != null)
                {
                    model.RunTimes += 1;
                    model.PerformTime = DateTime.Now;
                    model.Remark = $"【{DateTime.Now}】执行任务【Id：{context.JobDetail.Key.Name}，组别：{context.JobDetail.Key.Group}】【执行成功】{separator}"
                                 + string.Join(separator, StringHelper.GetTopDataBySeparator(model.Remark, separator, 9));

                    await _tasksQzSvc.Update(model);
                }
            }
        }

        /// <summary>
        /// 执行指定任务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="action"></param>
        public async Task<string> ExecuteJob(IJobExecutionContext context, Func<Task> func)
        {
            string jobHistory = $"【{DateTime.Now}】执行任务【Id：{context.JobDetail.Key.Name}，组别：{context.JobDetail.Key.Group}】";
            try
            {
                var s = context.Trigger.Key.Name;
                //记录Job时间
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                await func();//执行任务
                stopwatch.Stop();
                jobHistory += $"，【执行成功】，完成时间：{stopwatch.Elapsed.TotalMilliseconds.ToString("00")}毫秒";
            }
            catch (Exception ex)
            {
                _ = new JobExecutionException(ex)
                {
                    RefireImmediately = true //true  是立即重新执行任务
                };

                jobHistory += $"，【执行失败】，异常日志：{ex.Message}";
            }

            Console.Out.WriteLine(jobHistory);
            return jobHistory;
        }
    }
}