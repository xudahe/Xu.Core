﻿using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xu.IServices;
using Xu.Common;
using Xu.Model.Models;
using Xu.Model;

/// <summary>
/// 本月活跃用户（使用任务调度，1分钟统计一次）
/// </summary>
namespace Xu.Tasks
{
    public class Job_AccessTrendLog : JobBase, IJob
    {
        private readonly IAccessTrendLogSvc _accessTrendLogSvc;
        private readonly IWebHostEnvironment _environment;

        public Job_AccessTrendLog(IAccessTrendLogSvc accessTrendLogSvc, IWebHostEnvironment environment, ITasksQzSvc tasksQzSvc)
        {
            _accessTrendLogSvc = accessTrendLogSvc;
            _environment = environment;
            _tasksQzSvc = tasksQzSvc;
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

            var lastestLogDatetime = (await _accessTrendLogSvc.Query(null, d => d.ModifyTime, false)).FirstOrDefault()?.ModifyTime;
            if (lastestLogDatetime == null)
            {
                lastestLogDatetime = Convert.ToDateTime("2021-09-14");
            }

            var accLogs = GetAccessLogs().Where(d => d.User != "" && d.BeginTime.ToDateTimeReq() >= lastestLogDatetime).ToList();
            var logUpdate = DateTime.Now;

            var activeUsers = (from n in accLogs
                               group n by new { n.User } into g
                               select new ActiveUserVM
                               {
                                   User = g.Key.User,
                                   Count = g.Count(),
                               }).ToList();

            foreach (var item in activeUsers)
            {
                var user = (await _accessTrendLogSvc.Query(d => d.User != "" && d.User == item.User)).FirstOrDefault();
                if (user != null)
                {
                    user.Count += item.Count;
                    user.ModifyTime = logUpdate;
                    await _accessTrendLogSvc.Update(user);
                }
                else
                {
                    await _accessTrendLogSvc.Add(new AccessTrendLog()
                    {
                        Count = item.Count,
                        ModifyTime = logUpdate,
                        User = item.User
                    });
                }
            }

            // 重新拉取
            var actUsers = await _accessTrendLogSvc.Query(d => d.User != "", d => d.Count, false);
            actUsers = actUsers.Take(15).ToList();

            List<ActiveUserVM> activeUserVMs = new();
            foreach (var item in actUsers)
            {
                activeUserVMs.Add(new ActiveUserVM()
                {
                    User = item.User,
                    Count = item.Count
                });
            }

            Parallel.For(0, 1, e =>
            {
                LogLock.OutSql2Log("AccessTrendLog", new string[] { JsonConvert.SerializeObject(activeUserVMs) }, false, true);
            });
        }

        private List<UserAccessFromFIles> GetAccessLogs()
        {
            List<UserAccessFromFIles> userAccessModels = new();
            var accessLogs = LogLock.ReadLog(Path.Combine(_environment.ContentRootPath, "Log", DateTime.Now.ToString("yyyyMMdd")), "AccessTrendLog.log", Encoding.UTF8, ReadType.PrefixLatest, 2).ObjToString().TrimEnd(',');

            try
            {
                return JsonConvert.DeserializeObject<List<UserAccessFromFIles>>("[" + accessLogs + "]");
            }
            catch (Exception)
            {
                var accLogArr = accessLogs.Split("\n");
                foreach (var item in accLogArr)
                {
                    if (item.ObjToString() != "")
                    {
                        try
                        {
                            var accItem = JsonConvert.DeserializeObject<UserAccessFromFIles>(item.TrimEnd(','));
                            userAccessModels.Add(accItem);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return userAccessModels;
        }
    }

    public class UserAccessFromFIles
    {
        public string User { get; set; }
        public string IP { get; set; }
        public string API { get; set; }
        public string BeginTime { get; set; }
        public string OPTime { get; set; }
        public string RequestMethod { get; set; } = "";
        public string Agent { get; set; }
    }
}