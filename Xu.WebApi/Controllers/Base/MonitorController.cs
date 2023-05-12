using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Xu.Common;
using Xu.Model.ResultModel;
using Xu.Model.ViewModels;
using Xu.Services.IDS4Db;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 监控管理
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
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
        public object GetServerInfo()
        {
            return new MessageModel<ServerViewModel>()
            {
                Message = "获取成功",
                Success = true,
                Response = new ServerViewModel()
                {
                    //环境变量
                    EnvironmentName = _env.EnvironmentName,
                    //系统架构
                    OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                    //表示根目录
                    ContentRootPath = _env.ContentRootPath,
                    //表示根目录 + wwwroot
                    WebRootPath = _env.WebRootPath,
                    //.NET Core版本
                    FrameworkDescription = RuntimeInformation.FrameworkDescription,
                    //内存占用
                    MemoryFootprint = (Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + " MB",
                    //启动时间
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
            if (AppSettings.App(new string[] { "Middleware", "SignalRSendLog", "Enabled" }).ToBoolReq())
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

            if (AppSettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq())
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

        private static List<UserAccessModel> GetAccessLogsToday(IWebHostEnvironment environment)
        {
            List<UserAccessModel> userAccessModels = new();
            var accessLogs = LogLock.ReadLog(Path.Combine(environment.ContentRootPath, "Log"), "RecordAccessLogs", Encoding.UTF8, ReadType.Prefix, 2).ObjToString().TrimEnd(',');

            try
            {
                return JsonConvert.DeserializeObject<List<UserAccessModel>>("[" + accessLogs + "]");
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
                            var accItem = JsonConvert.DeserializeObject<UserAccessModel>(item.TrimEnd(','));
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

        private static List<ActiveUserVM> GetAccessLogsTrend(IWebHostEnvironment environment)
        {
            List<ActiveUserVM> userAccessModels = new();
            var accessLogs = LogLock.ReadLog(Path.Combine(environment.ContentRootPath, "Log"), "AccessTrendLog", Encoding.UTF8, ReadType.Prefix, 2).ObjToString().TrimEnd(',');

            try
            {
                return JsonConvert.DeserializeObject<List<ActiveUserVM>>(accessLogs);
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
                            var accItem = JsonConvert.DeserializeObject<ActiveUserVM>(item.TrimStart('[').TrimEnd(']'));
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

        /// <summary>
        /// 活跃用户
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<WelcomeInitData> GetActiveUsers([FromServices] IWebHostEnvironment environment)
        {
            var accessLogsToday = GetAccessLogsToday(environment).Where(d => d.BeginTime.ToDateTimeReq() >= DateTime.Today);

            var Logs = accessLogsToday.OrderByDescending(d => d.BeginTime).Take(50).ToList();

            var errorCountToday = LogLock.GetLogData().Where(d => d.Import == 9).Count();

            accessLogsToday = accessLogsToday.Where(d => d.User != "").ToList();

            var activeUsers = (from n in accessLogsToday
                               group n by new { n.User } into g
                               select new ActiveUserVM
                               {
                                   User = g.Key.User,
                                   Count = g.Count(),
                               }).ToList();

            int activeUsersCount = activeUsers.Count;
            activeUsers = activeUsers.OrderByDescending(d => d.Count).Take(10).ToList();

            return new MessageModel<WelcomeInitData>()
            {
                Message = "获取成功",
                Success = true,
                Response = new WelcomeInitData()
                {
                    ActiveUsers = activeUsers,
                    ActiveUserCount = activeUsersCount,
                    ErrorCount = errorCountToday,
                    Logs = Logs,
                    ActiveCount = GetAccessLogsTrend(environment)
                }
            };
        }
    }

    public class WelcomeInitData
    {
        /// <summary>
        /// 今日活跃用户
        /// </summary>
        public List<ActiveUserVM> ActiveUsers { get; set; }

        /// <summary>
        /// 今日活跃用户数量
        /// </summary>
        public int ActiveUserCount { get; set; }

        /// <summary>
        /// 访问日志
        /// </summary>
        public List<UserAccessModel> Logs { get; set; }

        /// <summary>
        /// 今日异常数量
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// 本月活跃用户
        /// </summary>
        public List<ActiveUserVM> ActiveCount { get; set; }
    }
}