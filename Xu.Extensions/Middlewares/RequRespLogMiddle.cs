using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.Extensions.Middlewares
{
    /// <summary>
    /// 中间件
    /// 记录请求和响应数据
    /// </summary>
    public class RequRespLogMiddle
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequRespLogMiddle> _logger;

        /// <summary>
        ///构造函数
        /// </summary>
        /// <param name="next"></param>
        public RequRespLogMiddle(RequestDelegate next, ILogger<RequRespLogMiddle> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (Appsettings.App("Middleware", "RequestResponseLog", "Enabled").ToBoolReq())
            {
                // 过滤，只有接口
                if (context.Request.Path.Value.Contains("api"))
                {
                    context.Request.EnableBuffering();
                    Stream originalBody = context.Response.Body;

                    try
                    {
                        // 存储请求数据
                        await RequestDataLog(context);

                        using (var ms = new MemoryStream())
                        {
                            context.Response.Body = ms;

                            await _next(context);

                            // 存储响应数据
                            ResponseDataLog(context.Response, ms);

                            ms.Position = 0;
                            await ms.CopyToAsync(originalBody);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录异常
                        //ErrorLogData(context.Response, ex);
                        _logger.LogError(ex.Message + "" + ex.InnerException);
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

        /// <summary>
        /// 存储请求数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task RequestDataLog(HttpContext context)
        {
            var request = context.Request;
            var sr = new StreamReader(request.Body);

            var content = $" QueryData：{request.Path + request.QueryString}\r\n BodyData：{await sr.ReadToEndAsync()}";

            if (!string.IsNullOrEmpty(content))
            {
                //Parallel.For(0, 1, e =>
                //{
                //    LogLock.OutSql2Log("RequestResponseLog", new string[] { "Request Data:", content });
                //});
                SerilogServer.WriteLog("RequestLog", new string[] { "Request Data：", content });

                request.Body.Position = 0;
            }
        }

        /// <summary>
        /// 存储响应数据
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ms"></param>
        private void ResponseDataLog(HttpResponse response, MemoryStream ms)
        {
            ms.Position = 0;
            var ResponseBody = new StreamReader(ms).ReadToEnd();

            // 去除 Html
            var reg = "<[^>]+>";
            var isHtml = Regex.IsMatch(ResponseBody, reg);

            if (!string.IsNullOrEmpty(ResponseBody))
            {
                //Parallel.For(0, 1, e =>
                //{
                //    LogLock.OutSql2Log("RequestResponseLog", new string[] { "Response Data:", ResponseBody });
                //});
                SerilogServer.WriteLog("ResponseLog", new string[] { "Response Data：", ResponseBody });
            }
        }
    }
}