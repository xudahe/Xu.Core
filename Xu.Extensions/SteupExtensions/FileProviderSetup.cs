using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// 将真实物理路径注册为虚拟路径
    /// </summary>
    public static class FileProviderSetup
    {
        public static void AddFileProviderSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Configure<VirtualPathConfig>(configuration);

            // var config1 = configuration.Get<VirtualPathConfig>().VirtualPath; //方法一
            var config = AppSettings.App<VirtualPath>("VirtualPath").ToList(); //方法二
            config.ForEach(d =>
            {
                services.AddSingleton(new FileProvider(d.RealPath, d.Alias));
            });
        }
    }
}