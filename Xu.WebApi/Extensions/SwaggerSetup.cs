﻿using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Linq;
using Xu.Common;
using static Xu.WebApi.CustomApiVersion;

namespace Xu.WebApi
{
    /// <summary>
    /// Swagger 启动服务
    /// </summary>
    public static class SwaggerSetup
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GlobalExceptionsFilter));

        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //获取项目路径
            var basePath = AppContext.BaseDirectory;
            //var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            var ApiName = Appsettings.App(new string[] { "Startup", "ApiName" });

            services.AddSwaggerGen(c =>
            {
                //遍历出全部的版本，做文档信息展示
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        //Version = version,
                        //Title = $"{ApiName} 接口文档——Netcore 3.1",
                        //Description = $"{ApiName} HTTP API " + version,
                        //Contact = new OpenApiContact { Name = ApiName, Email = "Xu.Core@xxx.com", Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") },
                        //License = new OpenApiLicense { Name = ApiName, Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") }
                    });
                    c.OrderActionsBy(o => o.RelativePath);
                    c.CustomSchemaIds(o => o.FullName); // 解决相同类名会报错的问题
                });

                try
                {
                    var xmlPath = Path.Combine(basePath, "Xu.WebApi.xml");//获取注入项目绝对路径，这个就是刚刚配置的xml文件名
                    c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                    var xmlModelPath = Path.Combine(basePath, "Xu.Model.xml");
                    c.IncludeXmlComments(xmlModelPath);
                }
                catch (Exception ex)
                {
                    log.Error("Xu.Core.xml 或 Xu.Model.xml丢失，请重新生成。\n" + ex.Message);
                }

                // 开启加权小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // 在header中添加token，传递到后台
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                // 必须是 oauth2
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
            });
        }
    }
}