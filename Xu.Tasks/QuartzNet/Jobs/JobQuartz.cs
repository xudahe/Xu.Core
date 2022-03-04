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

/// <summary>
/// 这里要注意下，命名空间和程序集是一样的，不然反射不到
/// </summary>
namespace Xu.Tasks
{
    public class JobQuartz : JobBase, IJob
    {
        // private readonly IBlogArticleServices _blogArticleServices;
        private readonly IWebHostEnvironment _environment;

        public JobQuartz(ITasksQzSvc tasksQzSvc, IWebHostEnvironment environment)
        {
            // _blogArticleServices = blogArticleServices;
            _environment = environment;
            _tasksQzSvc = tasksQzSvc;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var executeLog = await ExecuteJob(context, async () => await Run(context));
        }

        public async Task Run(IJobExecutionContext context)
        {
            // var list = await _blogArticleServices.Query();
            // 也可以通过数据库配置，获取传递过来的参数
            JobDataMap data = context.JobDetail.JobDataMap;
            //int jobId = data.GetInt("JobParam");

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
                //var logsIds = await _operateLogServices.Add(operateLogs);
            }
        }
    }
}