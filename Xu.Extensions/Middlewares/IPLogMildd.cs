using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.Extensions
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
            if (Appsettings.App("Middleware", "IPLog", "Enabled").ToBoolReq())
            {
                // 过滤，只有接口
                if (context.Request.Path.Value.Contains("api"))
                {
                    //reuqest支持buff,否则body只能读取一次
                    context.Request.EnableBuffering();

                    try
                    {
                        // 存储请求数据
                        var request = context.Request;
                        var requestInfo = JsonConvert.SerializeObject(new RequestInfo()
                        {
                            ClientIP = GetClientIP(context)?.Replace("::ffff:", ""),
                            Url = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase + context.Request.Path,
                            Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Week = DateHelper.GetWeek(),
                        });

                        if (!string.IsNullOrEmpty(requestInfo))
                        {
                            // 自定义log输出
                            Parallel.For(0, 1, e =>
                            {
                                LogLock.OutSql2Log("RequestIpInfoLog", new string[] { requestInfo + "," }, false);
                            });

                            // 这里读取过body  Position是读取过几次  而此操作优于控制器先行 控制器只会读取Position为零次的
                            request.Body.Position = 0;
                        }

                        await _next(context); // 执行下一个中间件
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

        /// <summary>
        /// 获取客户端ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIP(HttpContext context)
        {
            //该方法可以正常获取到IP地址
            string remoteIpAddress = context.Connection.RemoteIpAddress.ToString();
            //如果有使用Nginx做反向代理的话，使用上面的方式获取到的IP会是127.0.0.1，无法获取到真实的IP地址，则应该使用下面的方式
            if (context.Request.Headers.ContainsKey("X-Real-IP"))
            {
                string realIP = context.Request.Headers["X-Real-IP"].ToString();
                if (realIP != remoteIpAddress)
                {
                    remoteIpAddress = realIP;
                }
            }
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                string forwarded = context.Request.Headers["X-Forwarded-For"].ToString();
                if (forwarded != remoteIpAddress)
                {
                    remoteIpAddress = forwarded;
                }
            }
            return remoteIpAddress;
        }
    }
}