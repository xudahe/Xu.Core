using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// 中间件
    /// SignalR发送数据
    /// </summary>
    public class SignalRSendMildd
    {
        private readonly RequestDelegate _next;

        private readonly IHubContext<ChatHub> _hubContext;

        /// <summary>
        ///构造函数
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
            if (Appsettings.App("Middleware", "SignalR", "Enabled").ToBoolReq())
            {
                // 给当前连接到 Hub 上的所有连接发送消息，相当于广播
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData());
            }
            await _next(context);
        }
    }
}