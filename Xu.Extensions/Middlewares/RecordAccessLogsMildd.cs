﻿using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// 中间件
    /// 记录用户方访问数据
    /// </summary>
    public class RecordAccessLogsMildd
    {
        private readonly RequestDelegate _next;
        private readonly IAspNetUser _user;
        private readonly ILogger<RecordAccessLogsMildd> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next"></param>
        public RecordAccessLogsMildd(RequestDelegate next, IAspNetUser user, ILogger<RecordAccessLogsMildd> logger)
        {
            _next = next;
            _user = user;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (Appsettings.App("Middleware", "RecordAccessLogs", "Enabled").ToBoolReq())
            {
                // 过滤，只有接口
                if (context.Request.Path.Value.Contains("api"))
                {
                    //记录Job时间
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    context.Request.EnableBuffering();
                    Stream originalBody = context.Response.Body;

                    try
                    {
                        using (var ms = new MemoryStream())
                        {
                            context.Response.Body = ms;

                            await _next(context);

                            ms.Position = 0;
                            await ms.CopyToAsync(originalBody);

                            stopwatch.Stop();
                            var opTime = stopwatch.Elapsed.TotalMilliseconds.ToString("00") + "ms";
                            // 存储请求数据
                            await RequestDataLog(context, opTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录异常
                        //ErrorLogData(context.Response, ex);
                        _logger.LogError(ex.Message + "\r\n" + ex.InnerException);
                    }
                    finally
                    {
                        context.Response.Body = originalBody;
                    }
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task RequestDataLog(HttpContext context, string opTime)
        {
            var request = context.Request;
            var sr = new StreamReader(request.Body);

            var requestData = request.Method == "GET" || request.Method == "DELETE" ? HttpUtility.UrlDecode(request.QueryString.ToString(), Encoding.UTF8) : (await sr.ReadToEndAsync()).ToString();
            if (requestData.IsNullOrEmpty() && requestData.Length > 30)
            {
                requestData = requestData.Substring(0, 30);
            }

            var requestInfo = JsonConvert.SerializeObject(new UserAccessModel()
            {
                User = _user.Name,
                ClientIP = IPLogMildd.GetClientIP(context)?.Replace("::ffff:", ""),
                ServiceIP = context.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + context.Connection.LocalPort,
                Url = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase + context.Request.Path,
                BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                OPTime = opTime,
                RequestMethod = request.Method,
                RequestData = requestData,
                Agent = request.Headers["User-Agent"].ToString()
            });

            if (!string.IsNullOrEmpty(requestInfo))
            {
                // 自定义log输出
                Parallel.For(0, 1, e =>
                {
                    LogLock.OutSql2Log("RecordAccessLogs", new string[] { requestInfo + "," }, false);
                });

                request.Body.Position = 0;
            }
        }
    }
}