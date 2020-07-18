using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xu.Common;
using Xu.Extensions;
using Xu.Model.ResultModel;
using Xu.Model.ViewModels;

namespace Blog.Core.Controllers
{
    /// <summary>
    /// 监控管理
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(Permissions.Name)]
    public class MonitorController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment _env;

        public MonitorController(IHubContext<ChatHub> hubContext, IWebHostEnvironment env)
        {
            _hubContext = hubContext;
            _env = env;
        }

        /// <summary>
        /// 服务器配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object Server()
        {
            return new MessageModel<ServerViewModel>()
            {
                Message = "获取成功",
                Success = true,
                Response = new ServerViewModel()
                {
                    EnvironmentName = _env.EnvironmentName,
                    OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                    ContentRootPath = _env.ContentRootPath,
                    WebRootPath = _env.WebRootPath,
                    FrameworkDescription = RuntimeInformation.FrameworkDescription,
                    MemoryFootprint = (Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + " MB",
                    WorkingTime = DateHelper.TimeSubTract(DateTime.Now, Process.GetCurrentProcess().StartTime)
                }
            };
        }

        /// <summary>
        /// SignalR send data
        /// </summary>
        /// <returns></returns>
        // GET: api/Logs
        [HttpGet]
        public object Get()
        {
            _hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData()).Wait();

            return new MessageModel<List<LogInfo>>()
            {
                Message = "获取成功",
                Success = true,
                Response = null
            };
        }

        [HttpGet]
        public object GetAccessApiByWeek()
        {
            return new MessageModel<RequestApiWeekView>()
            {
                Message = "获取成功",
                Success = true,
                Response = LogLock.AccessApiByWeek()
            };
        }

        [HttpGet]
        public object GetAccessApiByDate()
        {
            return new MessageModel<AccessApiDateView>()
            {
                Message = "获取成功",
                Success = true,
                Response = LogLock.AccessApiByDate()
            };
        }

        [HttpGet]
        public object GetAccessApiByHour()
        {
            return new MessageModel<AccessApiDateView>()
            {
                Message = "获取成功",
                Success = true,
                Response = LogLock.AccessApiByHour()
            };
        }

        /// <summary>
        /// 记录访问日志
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetAccessLogs([FromServices] IWebHostEnvironment environment)
        {
            var Logs = JsonConvert.DeserializeObject<List<UserAccessModel>>("[" + LogLock.ReadLog(Path.Combine(environment.ContentRootPath, "Log", "RecordAccessLogs.log"), Encoding.UTF8) + "]");

            Logs = Logs.Where(d => d.BeginTime.ToDateTime() >= DateTime.Today).OrderByDescending(d => d.BeginTime).Take(50).ToList();
            return new MessageModel<List<UserAccessModel>>()
            {
                Message = "获取成功",
                Success = true,
                Response = Logs
            };
        }
    }
}