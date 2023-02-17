using Microsoft.Extensions.DependencyInjection;
using System;
using Xu.Common;

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
                if (!AppSettings.App(new string[] { "Startup", "Cors", "EnableAllIPs" }).ToBoolReq())
                {
                    c.AddPolicy(AppSettings.App(new string[] { "Startup", "Cors", "PolicyName" }),
                        policy =>
                        {
                            policy
                            .WithOrigins(AppSettings.App(new string[] { "Startup", "Cors", "IPs" }).Split(','))
                            .AllowAnyHeader()//Ensures that the policy allows any header.
                            .AllowAnyMethod();
                        });
                }
                else
                {
                    //允许任意跨域请求
                    c.AddPolicy(AppSettings.App(new string[] { "Startup", "Cors", "PolicyName" }),
                        policy =>
                        {
                            policy
                            .SetIsOriginAllowed((host) => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                        });
                }

                //.WithOrigins(AppSettings.App(new string[] { "Startup", "Cors", "IPs" }).Split(',')) //允许部分站点跨域请求
                //.SetPreflightMaxAge(TimeSpan.FromSeconds(2520)) //添加预检请求过期时间
                //.SetIsOriginAllowed(origin => true) //允许任何域
                //.AllowAnyHeader()  //允许任何头
                //.AllowAnyMethod() //允许任何方式
                //.AllowCredentials(); // 允许Cookie信息
            });
        }
    }
}