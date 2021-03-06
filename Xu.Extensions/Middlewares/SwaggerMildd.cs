﻿using log4net;
using Microsoft.AspNetCore.Builder;
using System;
using System.IO;
using System.Linq;
using Xu.Common;
using static Xu.Extensions.CustomApiVersion;

namespace Xu.Extensions
{
    /// <summary>
    /// Swagger中间件
    /// </summary>
    public static class SwaggerMildd
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SwaggerMildd));

        public static void UseSwaggerMildd(this IApplicationBuilder app, Func<Stream> streamHtml)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //根据版本名称倒序 遍历展示
                var ApiName = Appsettings.App(new string[] { "Startup", "ApiName" });
                typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });

                // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：{项目名.index.html}
                if (streamHtml.Invoke() == null)
                {
                    var msg = "index.html的属性，必须设置为嵌入的资源";
                    log.Error(msg);
                    throw new Exception(msg);
                }
                c.IndexStream = streamHtml;

                c.DefaultModelsExpandDepth(-1); // 不显示models

                c.RoutePrefix = "";//路径配置，设置后直接输入IP就可以进入接口文档
            });
        }
    }
}