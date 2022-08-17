using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;

namespace Xu.Tasks
{
    /// <summary>
    /// 封装基础类
    /// </summary>
    public class JobBase
    {
        public ITasksQzSvc _tasksQzSvc;

        /// <summary>
        /// 执行指定任务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="action"></param>
        public async Task<string> ExecuteJob(IJobExecutionContext context, Func<Task> func)
        {
            //记录Job时间
            Stopwatch stopwatch = new Stopwatch();
            //JOBID
            int jobid = context.JobDetail.Key.Name.ToInt32Req();
            //JOB组名
            string groupName = context.JobDetail.Key.Group;
            //日志
            string jobHistory = $"【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行开始】【Id：{jobid}，组别：{groupName}】";
            //耗时
            double taskSeconds = 0;
            try
            {
                stopwatch.Start();
                await func();//执行任务
                stopwatch.Stop();
                jobHistory += $"，【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行成功】";
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                e2.RefireImmediately = true; //true 是立即重新执行任务
                jobHistory += $"，【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行失败:{ex.Message}】";
            }
            finally
            {
                taskSeconds = Math.Round(stopwatch.Elapsed.TotalSeconds, 3);  // 总秒数
                jobHistory += $"，【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行结束】(耗时:{taskSeconds}秒)";
                if (_tasksQzSvc != null)
                {
                    var separator = "<br>";
                    var model = await _tasksQzSvc.QueryById(jobid);
                    if (model != null)
                    {
                        model.RunTimes += 1;
                        model.PerformTime = DateTime.Now;

                        // 这里注意数据库字段的长度问题，超过限制，会造成数据库remark不更新问题。
                        model.TasksLog = $"{jobHistory}{separator}" + string.Join(separator, StringHelper.GetTopDataBySeparator(model.TasksLog, separator, 5));
                        await _tasksQzSvc.Update(model);

                        SerilogServer.WriteLog("任务调度--" + model.JobName, new string[] { jobHistory }, false);
                    }
                }
            }

            Console.Out.WriteLine(jobHistory);
            return jobHistory;
        }
    }
}