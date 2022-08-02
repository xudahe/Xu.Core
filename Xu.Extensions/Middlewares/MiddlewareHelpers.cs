﻿using Microsoft.AspNetCore.Builder;

namespace Xu.Extensions
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
            return app.UseMiddleware<JwtTokenAuth>();
        }

        /// <summary>
        /// 请求响应中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseReuestResponseLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequRespLogMildd>();
        }

        /// <summary>
        /// SignalR中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSignalRSendMildd(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SignalRSendMildd>();
        }

        /// <summary>
        /// 异常处理中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandlerMidd(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlerMidd>();
        }

        /// <summary>
        /// IP请求中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIPLogMildd(this IApplicationBuilder app)
        {
            return app.UseMiddleware<IPLogMildd>();
        }

        /// <summary>
        /// 用户访问中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRecordAccessLogsMildd(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RecordAccessLogsMildd>();
        }
    }
}