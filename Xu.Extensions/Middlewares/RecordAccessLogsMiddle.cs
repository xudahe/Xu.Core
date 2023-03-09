using Microsoft.AspNetCore.Hosting;
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
using Xu.Common.HttpContextUser;
using Xu.IServices;

namespace Xu.Extensions.Middlewares
{
    /// <summary>
    /// 中间件
    /// 记录用户方访问数据
    /// </summary>
    public class RecordAccessLogsMiddle
    {
        private readonly RequestDelegate _next;
        private readonly IAspNetUser _user;
        private readonly ILogger<RecordAccessLogsMiddle> _logger;
        private readonly IWebHostEnvironment _environment;
        private Stopwatch _stopwatch;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next"></param>
        public RecordAccessLogsMiddle(RequestDelegate next, IAspNetUser user, ILogger<RecordAccessLogsMiddle> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _user = user;
            _logger = logger;
            _environment = environment;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (AppSettings.App("Middleware", "RecordAccessLogs", "Enabled").ToBoolReq())
            {
                var api = context.Request.Path.ToString().TrimEnd('/').ToLower();
                var ignoreApis = AppSettings.App("Middleware", "RecordAccessLogs", "IgnoreApis"); //忽略的接口

                // 过滤，只有接口
                if (api.Contains("api") && !ignoreApis.Contains(api))
                {
                    _stopwatch.Restart();

                    HttpRequest request = context.Request;

                    UserAccessModel userAccessModel = new UserAccessModel();

                    userAccessModel.Api = api;
                    userAccessModel.User = _user.Name;
                    userAccessModel.ClientIP = IPLogMiddle.GetClientIP(context)?.Replace("::ffff:", "");
                    userAccessModel.ServiceIP = context.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + context.Connection.LocalPort;
                    userAccessModel.Url = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase + context.Request.Path;
                    userAccessModel.BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    userAccessModel.RequestMethod = request.Method;
                    userAccessModel.Agent = request.Headers["User-Agent"].ToString();

                    // 获取请求body内容
                    if (request.Method.ToLower().Equals("post") || request.Method.ToLower().Equals("put"))
                    {
                        // 启用倒带功能，就可以让 Request.Body 可以再次读取
                        request.EnableBuffering();

                        Stream stream = request.Body;
                        byte[] buffer = new byte[request.ContentLength.Value];
                        stream.Read(buffer, 0, buffer.Length);
                        userAccessModel.RequestData = Encoding.UTF8.GetString(buffer);

                        request.Body.Position = 0;
                    }
                    else if (request.Method.ToLower().Equals("get") || request.Method.ToLower().Equals("delete"))
                    {
                        userAccessModel.RequestData = HttpUtility.UrlDecode(request.QueryString.ToString(), Encoding.UTF8);
                    }

                    // 获取Response.Body内容
                    var originalBodyStream = context.Response.Body;
                    using (var responseBody = new MemoryStream())
                    {
                        context.Response.Body = responseBody;

                        await _next(context);

                        var responseBodyData = await GetResponse(context.Response);

                        await responseBody.CopyToAsync(originalBodyStream);
                    }

                    // 响应完成记录时间和存入日志
                    context.Response.OnCompleted(() =>
                    {
                        _stopwatch.Stop();

                        userAccessModel.OPTime = _stopwatch.ElapsedMilliseconds + "ms";

                        // 自定义log输出
                        var requestInfo = JsonConvert.SerializeObject(userAccessModel);
                        Parallel.For(0, 1, e =>
                        {
                            LogLock.OutLogAOP("RecordAccessLogs", context.TraceIdentifier, new string[] { userAccessModel.GetType().ToString(), requestInfo }, false);
                        });
                        //SerilogServer.WriteLog("RecordAccessLogs", new string[] { userAccessModel.GetType().ToString(), requestInfo }, false);

                        return Task.CompletedTask;
                    });
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
        /// 获取响应内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<string> GetResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }
}