using Microsoft.Extensions.DependencyInjection;
using System;

namespace Xu.Extensions
{
    /// <summary>
    /// MiniProfiler 启动服务（接口执行时间分析）
    /// </summary>
    public static class MiniProfilerSetup
    {
        public static void AddMiniProfilerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // 3.x使用MiniProfiler，必须要注册MemoryCache服务
            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler"; //设定访问分析结果URL的路由基地址，注意这个路径要和下边 index.html 脚本配置中的一致
                //(options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(10);
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.Left; //设定弹出窗口的位置是左上角
                options.PopupShowTimeWithChildren = true; //设定在弹出的明细窗口里会显式Time With Children这列

                // 可以增加权限
                //options.ResultsAuthorize = request => request.HttpContext.User.IsInRole("Admin");
                //options.UserIdProvider = request => request.HttpContext.User.Identity.Name;
            }
           );
        }
    }
}