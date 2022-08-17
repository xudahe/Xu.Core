using Microsoft.AspNetCore.Builder;

namespace Xu.Extensions.Middlewares
{
    //这里定义一个中间件Helper，主要作用就是给当前模块的中间件取一个别名
    public static class MiddlewareHelpers
    {
        /// <summary>
        /// 自定义授权中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtTokenAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtTokenAuthMiddle>();
        }

        /// <summary>
        /// 请求响应中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequRespLogMiddle(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequRespLogMiddle>();
        }

        /// <summary>
        /// SignalR中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSignalRSendMiddle(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SignalRSendMiddle>();
        }

        /// <summary>
        /// 异常处理中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandlerMiddle(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlerMiddle>();
        }

        /// <summary>
        /// IP请求中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIpLogMiddle(this IApplicationBuilder app)
        {
            return app.UseMiddleware<IPLogMiddle>();
        }

        /// <summary>
        /// 用户访问中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRecordAccessLogsMiddle(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RecordAccessLogsMiddle>();
        }
    }
}