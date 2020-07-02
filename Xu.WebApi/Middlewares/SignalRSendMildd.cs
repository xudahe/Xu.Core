using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SqlSugar;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.WebApi
{
    /// <summary>
    /// 中间件
    /// 记录请求和响应数据
    /// </summary>
    public class SignalRSendMildd
    {
        /// <summary>
        ///
        /// </summary>
        private readonly RequestDelegate _next;

        private readonly IHubContext<ChatHub> _hubContext;

        /// <summary>
        ///
        /// </summary>
        /// <param name="next"></param>
        /// <param name="hubContext"></param>
        public SignalRSendMildd(RequestDelegate next, IHubContext<ChatHub> hubContext)
        {
            _next = next;
            _hubContext = hubContext;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (Appsettings.App("Middleware", "SignalR", "Enabled").ObjToBool())
            {
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData());
            }
            await _next(context);
        }
    }
}