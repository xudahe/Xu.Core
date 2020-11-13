using Microsoft.Extensions.DependencyInjection;
using System;
using WebApiClient.Extensions.DependencyInjection;
using Xu.IServices.WebApiClients;

namespace Xu.Extensions
{
    /// <summary>
    /// WebApiClientSetup 启动服务
    /// </summary>
    public static class WebApiClientSetup
    {
        /// <summary>
        /// 注册WebApiClient 第三方接口
        /// </summary>
        /// <param name="services"></param>
        public static void AddHttpApi(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHttpApi<IDoubanApi>().ConfigureHttpApiConfig(c =>
            {
                c.HttpHost = new Uri("http://api.xiaomafeixiang.com/");
                c.FormatOptions.DateTimeFormat = WebApiClient.DateTimeFormats.ISO8601_WithMillisecond;
            });
        }
    }
}