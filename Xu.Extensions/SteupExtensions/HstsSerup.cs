using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xu.Extensions
{
    /// <summary>
    /// UseHsts 启动服务
    /// </summary>
    /// <remarks>
    /// 不建议在开发中使用，因为 HSTS 设置通过浏览器高度可缓存。 默认情况下，不 UseHsts 包括本地环回地址。
    /// </remarks>
    public static class HstsSetup
    {
        public static void AddHstsSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //HSTS 中间件（UseHsts）用于向客户端发送 HTTP 严格传输安全协议（HSTS）标头
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true; //是否包含子域名，默认false
                options.MaxAge = TimeSpan.FromDays(30);  //有效时长，默认30天
                options.ExcludedHosts.Add("example.com"); // 添加 example.com 到要排除的主机列表
            });

            //注入HTTPS服务,并设置管道
            services.AddHttpsRedirection(options =>
            {
                //中间件默认为通过所有重定向发送Status307TemporaryRedirect 
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect; 
                options.HttpsPort = 5001;
            });

        }
    }
}
