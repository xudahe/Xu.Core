using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xu.Common.Core;
using Xu.Common.HttpContextUser;

namespace Xu.Common;

public class App
{
    public static IServiceProvider RootServices => InternalApp.RootServices ;

    /// <summary>
    /// 获取请求上下文
    /// </summary>
    public static HttpContext HttpContext => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext;

    public static IAspNetUser User => HttpContext == null ? null : RootServices?.GetService<IAspNetUser>();
}