using Microsoft.Extensions.DependencyInjection;
using System;

namespace Xu.Extensions
{
    /// <summary>
    /// Cors 启动服务
    /// </summary>
    public static class CorsSetup
    {
        /// <summary>
        /// Cors 跨域资源共享
        /// </summary>
        /// <param name="services"></param>
        public static void AddCorsSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCors(c =>
            {
                c.AddPolicy("LimitRequests", policy =>
                {
                    // 支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
                    // 注意，http://127.0.0.1:1818 和 http://localhost:1818 是不一样的，尽量写两个
                    policy
                    //.WithOrigins(Appsettings.App(new string[] { "Startup", "Cors", "IPs" }).Split(',')) //允许部分站点跨域请求
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(2520)) //添加预检请求过期时间
                    .SetIsOriginAllowed(origin => true) //允许任何域
                    .AllowAnyHeader()  //允许任何头
                    .AllowAnyMethod() //允许任何方式
                    .AllowCredentials(); // 允许Cookie信息
                });
            });
        }
    }
}