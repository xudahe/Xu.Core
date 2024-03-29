﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Profiling;
using Xu.Common;
using Xu.Model.ResultModel;

namespace Xu.WebApi
{
    /// <summary>
    /// 全局异常错误日志
    /// </summary>
    public class GlobalExceptionsFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<GlobalExceptionsFilter> _loggerHelper;

        public GlobalExceptionsFilter(IWebHostEnvironment env, ILogger<GlobalExceptionsFilter> loggerHelper, IHubContext<ChatHub> hubContext)
        {
            _env = env;
            _loggerHelper = loggerHelper;
            _hubContext = hubContext;
        }

        public void OnException(ExceptionContext context)
        {
            var json = new MessageModel<string>
            {
                Message = context.Exception.Message,//错误信息
                Status = 500//500异常
            };
            var errorAudit = "Unable to resolve service for";
            if (!string.IsNullOrEmpty(json.Message) && json.Message.Contains(errorAudit))
            {
                json.Message = json.Message.Replace(errorAudit, $"（若新添加服务，需要重新编译项目）{errorAudit}");
            }

            if (_env.EnvironmentName.ObjToString().Equals("Development"))
            {
                json.MessageDev = context.Exception.StackTrace;//堆栈信息
            }
            context.Result = new ContentResult
            {
                Content = JsonHelper.GetJSON<MessageModel<string>>(json)
            };

            MiniProfiler.Current.CustomTiming("Errors：", json.Message);

            //采用log4net 进行错误日志记录
            _loggerHelper.LogError(json.Message + WriteLog(json.Message, context.Exception));
            //每次更新日志，推送到客户端——实时短信
            if (AppSettings.App(new string[] { "Middleware", "SignalRSendLog", "Enabled" }).ToBoolReq())
            {
                _hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData()).Wait();
            }
        }

        /// <summary>
        /// 自定义返回格式
        /// </summary>
        /// <param name="throwMsg"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public string WriteLog(string throwMsg, Exception ex)
        {
            return string.Format("\r\n【自定义错误】：{0} \r\n【异常类型】：{1} \r\n【异常信息】：{2} \r\n【堆栈调用】：{3}", new object[] { throwMsg,
                ex.GetType().Name, ex.Message, ex.StackTrace });
        }
    }

    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value) : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    //返回错误信息
    public class JsonErrorResponse
    {
        /// <summary>
        /// 生产环境的消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 开发环境的消息
        /// </summary>
        public string DevelopmentMessage { get; set; }
    }
}