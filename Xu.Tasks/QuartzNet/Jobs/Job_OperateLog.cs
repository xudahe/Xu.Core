using Microsoft.AspNetCore.Hosting;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Tasks
{
    /// <summary>
    /// 操作日志记录
    /// </summary>
    public class Job_OperateLog : JobBase, IJob
    {
        private readonly IOperateLogSvc _operateLogSvc;
        private readonly IWebHostEnvironment _environment;

        public Job_OperateLog(IOperateLogSvc operateLogSvc, IWebHostEnvironment environment)
        {
            _operateLogSvc = operateLogSvc;
            _environment = environment;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var executeLog = await ExecuteJob(context, async () => await Run(context));
        }

        public async Task Run(IJobExecutionContext context)
        {
            // 可以直接获取 JobDetail 的值
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            // 也可以通过数据库配置，获取传递过来的参数
            JobDataMap data = context.JobDetail.JobDataMap;

            List<LogInfo> excLogs = new List<LogInfo>();
            var exclogContent = LogLock.ReadLog(Path.Combine(_environment.ContentRootPath, "Log"), $"GlobalExceptionLogs_{DateTime.Now.ToString("yyyMMdd")}.log", Encoding.UTF8);

            if (!string.IsNullOrEmpty(exclogContent))
            {
                excLogs = exclogContent.Split("--------------------------------")
                             .Where(d => !string.IsNullOrEmpty(d) && d != "\n" && d != "\r\n")
                             .Select(d => new LogInfo
                             {
                                 Datetime = (d.Split("|")[0]).Split(',')[0].ToDateTimeReq(),
                                 Content = d.Split("|")[1]?.Replace("\r\n", "<br>"),
                                 LogColor = "EXC",
                                 Import = 9,
                             }).ToList();
            }

            var filterDatetime = DateTime.Now.AddHours(-1);
            excLogs = excLogs.Where(d => d.Datetime >= filterDatetime).ToList();

            var operateLogs = new List<OperateLog>() { };
            excLogs.ForEach(m =>
            {
                operateLogs.Add(new OperateLog()
                {
                    LogTime = m.Datetime,
                    Description = m.Content,
                    IPAddress = m.ClientIP,
                    UserId = 0,
                    IsDeleted = false,
                });
            });

            if (operateLogs.Count > 0)
            {
                var logsIds = await _operateLogSvc.Add(operateLogs);
            }
        }
    }
}