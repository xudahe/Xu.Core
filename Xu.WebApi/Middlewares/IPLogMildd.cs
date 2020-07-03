using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.WebApi
{
    /// <summary>
    /// 中间件
    /// 记录IP请求数据
    /// </summary>
    public class IPLogMildd
    {
        /// <summary>
        ///
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        ///
        /// </summary>
        /// <param name="next"></param>
        public IPLogMildd(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (Appsettings.App("Middleware", "IPLog", "Enabled").ObjToBool())
            {
                // 过滤，只有接口
                if (context.Request.Path.Value.Contains("api"))
                {
                    context.Request.EnableBuffering();

                    try
                    {
                        // 存储请求数据
                        var request = context.Request;
                        var requestInfo = JsonConvert.SerializeObject(new RequestInfo()
                        {
                            Ip = GetClientIP(context),
                            Url = request.Path.ObjToString().TrimEnd('/').ToLower(),
                            Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Date = DateTime.Now.ToString("yyyy-MM-dd"),
                            Week = GetWeek(),
                        });

                        if (!string.IsNullOrEmpty(requestInfo))
                        {
                            Parallel.For(0, 1, e =>
                            {
                                LogLock.OutSql2Log("RequestIpInfoLog", new string[] { requestInfo + "," }, false);
                            });

                            request.Body.Position = 0;
                        }

                        await _next(context);
                    }
                    catch (Exception)
                    {
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

        private string GetWeek()
        {
            return DateTime.Now.DayOfWeek switch
            {
                DayOfWeek.Monday => "周一",
                DayOfWeek.Tuesday => "周二",
                DayOfWeek.Wednesday => "周三",
                DayOfWeek.Thursday => "周四",
                DayOfWeek.Friday => "周五",
                DayOfWeek.Saturday => "周六",
                DayOfWeek.Sunday => "周日",
                _ => "N/A",
            };
        }

        public static string GetClientIP(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].ObjToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ObjToString();
            }
            return ip;
        }
    }
}