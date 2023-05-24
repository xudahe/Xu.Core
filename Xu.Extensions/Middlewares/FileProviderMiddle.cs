using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System;
using System.Linq;
using Xu.Common;

namespace Xu.Extensions.Middlewares
{
    /// <summary>
    ///
    /// </summary>
    public static class FileProviderMiddle
    {
        public static void UseFileProviderMiddle(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            var config = AppSettings.App<VirtualPath>("VirtualPath").ToList();
            config.ForEach(d =>
            {
                //注册wwwroot 以外的静态文件
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(d.RealPath),
                    RequestPath = d.RequestPath, //虚拟路径用来访问静态文件
                });

                //注册wwwroot 以外的文件目录结构
                app.UseDirectoryBrowser(new DirectoryBrowserOptions
                {
                    FileProvider = new PhysicalFileProvider(d.RealPath),
                    RequestPath = d.RequestPath, //虚拟路径用来访问静态文件
                });
            });
        }
    }
}