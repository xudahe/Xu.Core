﻿using log4net;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Xu.Common;
using Xu.EnumHelper;
using static Xu.Extensions.CustomApiVersion;

namespace Xu.Extensions
{
    /// <summary>
    /// Swagger 启动服务
    /// </summary>
    public static class SwaggerSetup
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SwaggerSetup));

        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var basePath = AppContext.BaseDirectory;
            //var basePath2 = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            var ApiName = AppSettings.App(new string[] { "Startup", "ApiName" });

            services.AddSwaggerGen(c =>
            {
                //遍历出全部的版本，做文档信息展示
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Version = version,
                        Title = $"{ApiName} 接口文档——{RuntimeInformation.FrameworkDescription}",
                        Description = $"{ApiName} HTTP API " + version,
                        //Contact = new OpenApiContact { Name = ApiName, Email = "webApi@xxx.com", Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") },
                        //License = new OpenApiLicense { Name = ApiName + " 官方文档", Url = new Uri("http://apk.neters.club/.doc/") }
                    });
                    c.OrderActionsBy(o => o.RelativePath);
                    c.CustomSchemaIds(o => o.FullName); // 解决相同类名会报错的问题
                });

                try
                {
                    //这个就是刚刚配置的xml文件名
                    var xmlPath = Path.Combine(basePath, "Xu.WebApi.xml");
                    //默认的第二个参数是false，这个是controller的注释，记得修改
                    c.IncludeXmlComments(xmlPath, true);

                    //这个就是Model层的xml文件名
                    var xmlModelPath = Path.Combine(basePath, "Xu.Model.xml");
                    c.IncludeXmlComments(xmlModelPath);
                }
                catch (Exception ex)
                {
                    log.Error("Xu.WebApi.xml和Xu.Model.xml 丢失，请检查并拷贝。\n" + ex.Message);
                }

                // 开启加权小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // 在header中添加token，传递到后台
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.DocumentFilter<SwaggerHiddenApi>();

                // ids4和jwt切换
                if (Permissions.IsUseIds4)
                {
                    //接入identityserver4
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"{AppSettings.App(new string[] { "Startup", "IdentityServer4", "AuthorizationUrl" })}/connect/authorize"),
                                Scopes = new Dictionary<string, string> {
                                {
                                    "blog.core.api","ApiResource id"
                                }
                            }
                            }
                        }
                    });
                }
                else
                {
                    // Jwt Bearer 认证，必须是 oauth2
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                        Name = "Authorization",//jwt默认的参数名称
                        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                        Type = SecuritySchemeType.ApiKey
                    });
                }
            });
        }
    }

    /// <summary>
    /// 自定义版本
    /// </summary>
    public class CustomApiVersion
    {
        /// <summary>
        /// Api接口版本 自定义
        /// </summary>
        public enum ApiVersions
        {
            /// <summary>
            /// V1 版本
            /// </summary>
            [EnumText("V1")]
            V1 = 1,

            /// <summary>
            /// V2 版本
            /// </summary>
            [EnumText("V2")]
            V2 = 2,
        }
    }

    public class SwaggerHiddenApi : IDocumentFilter
    {
        /// <summary>
        /// 隐藏swagger接口特性标识，控制器上添加[SwaggerApi.HideApi]
        /// </summary>
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class HideApiAttribute : System.Attribute
        {
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (ApiDescription description in context.ApiDescriptions)
            {
                if (description.TryGetMethodInfo(out MethodInfo method))
                {
                    if (method.ReflectedType.CustomAttributes.Any(t => t.AttributeType == typeof(HideApiAttribute))
                            || method.CustomAttributes.Any(t => t.AttributeType == typeof(HideApiAttribute)))
                    {
                        string key = "/" + description.RelativePath;
                        if (key.Contains("?"))
                        {
                            int idx = key.IndexOf("?", System.StringComparison.Ordinal);
                            key = key.Substring(0, idx);
                        }
                        swaggerDoc.Paths.Remove(key);
                    }
                }
            }
        }
    }
}