using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xu.Common;
using Xu.Common.LogHelper;
using Xu.Model.ResultModel;
using Xu.Model.ViewModel;
using Xu.Services.IDS4Db;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 监控管理
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "v1")]
    //[Authorize(Permissions.Name)]
    public class MonitorController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationUserSvc _applicationUserSvc;

        public MonitorController(IHubContext<ChatHub> hubContext, IWebHostEnvironment env, IApplicationUserSvc applicationUserSvc)
        {
            _hubContext = hubContext;
            _env = env;
            _applicationUserSvc = applicationUserSvc;
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
        public MessageModel<List<LogInfo>> Get()
        {
            if (Appsettings.App(new string[] { "Middleware", "SignalRSendLog", "Enabled" }).ToBoolReq())
            {
                _hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData()).Wait();
            }
            return MessageModel<List<LogInfo>>.Msg(true, "执行成功");
        }

        /// <summary>
        /// Ip 请求日志
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetIpLogByDate()
        {
            return new MessageModel<List<LogInfo>>()
            {
                Message = "获取成功",
                Success = true,
                Response = LogLock.GetIpLogByDate()
            };
        }

        /// <summary>
        /// 获取前N条所有日志
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetLogInfo(int num = 200)
        {
            return new MessageModel<List<LogInfo>>()
            {
                Message = "获取成功",
                Success = true,
                Response = LogLock.GetLogData(num)
            };
        }

        /// <summary>
        /// Api请求访问日志
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetAccessLogs()
        {
            return new MessageModel<List<UserAccessModel>>()
            {
                Message = "获取成功",
                Success = true,
                Response = LogLock.GetAccessLogByDate()
            };
        }

        [HttpGet]
        public async Task<MessageModel<AccessApiDateView>> GetIds4Users()
        {
            List<ApiDate> apiDates = new List<ApiDate>();

            if (Appsettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq())
            {
                var users = await _applicationUserSvc.Query(d => d.TdIsDelete == false);

                apiDates = (from n in users
                            group n by new { n.Birth.Date } into g
                            select new ApiDate
                            {
                                Date = g.Key?.Date.ToString("yyyy-MM-dd"),
                                Count = g.Count(),
                            }).ToList();

                apiDates = apiDates.OrderByDescending(d => d.Date).Take(30).ToList();
            }

            if (apiDates.Count == 0)
            {
                apiDates.Add(new ApiDate()
                {
                    Date = "没数据,或未开启相应接口服务",
                    Count = 0
                });
            }
            return new MessageModel<AccessApiDateView>()
            {
                Message = "获取成功",
                Success = true,
                Response = new AccessApiDateView
                {
                    Columns = new string[] { "date", "count" },
                    Rows = apiDates.OrderBy(d => d.Date).ToList(),
                }
            };
        }

        [HttpGet]
        public MessageModel<RequestApiWeekView> GetRequestApiinfoByWeek()
        {
            return new MessageModel<RequestApiWeekView>()
            {
                Message = "获取成功",
                Success = true,
                Response = LogLock.RequestApiinfoByWeek()
            };
        }

        [HttpGet]
        public MessageModel<AccessApiDateView> GetAccessApiByDate()
        {
            return new MessageModel<AccessApiDateView>()
            {
                Message = "获取成功",
                Success = true,
                Response = LogLock.AccessApiByDate()
            };
        }

        [HttpGet]
        public MessageModel<AccessApiDateView> GetAccessApiByHour()
        {
            return new MessageModel<AccessApiDateView>()
            {
                Message = "获取成功",
                Success = true,
                Response = LogLock.AccessApiByHour()
            };
        }
    }
}