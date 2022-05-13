using Microsoft.Extensions.DependencyInjection;
using System;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// MiniProfiler 启动服务（接口执行时间分析）
    /// </summary>
    public static class MiniProfilerSetup
    {
        /// <summary>
        /// MiniProfiler 接口执行时间分析
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProfilerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (Appsettings.App(new string[] { "Startup", "MiniProfiler", "Enabled" }).ToBoolReq())
            {
                // 3.x使用MiniProfiler，必须要注册MemoryCache服务
                services.AddMiniProfiler(options =>
                {
                    //访问地址路由根目录；默认为：/mini-profiler-resources
                    options.RouteBasePath = "/profiler";
                    //数据缓存时间
                    //(options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);
                    //sql格式化设置
                    options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
                    //跟踪连接打开关闭
                    options.TrackConnectionOpenClose = true;
                    //界面主题颜色方案;默认浅色
                    options.ColorScheme = StackExchange.Profiling.ColorScheme.Light;
                    //.net core 3.0以上：对MVC过滤器进行分析
                    options.EnableMvcFilterProfiling = true;
                    //对视图进行分析
                    options.EnableMvcViewProfiling = true;

                    //设定弹出窗口的位置是左上角
                    options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.Left;
                    //设定在弹出的明细窗口里会显式Time With Children这列
                    options.PopupShowTimeWithChildren = true;

                    // 可以增加权限
                    //options.ResultsAuthorize = request => request.HttpContext.User.IsInRole("Admin");
                    //options.UserIdProvider = request => request.HttpContext.User.Identity.Name;
                }
               );
            }
        }
    }
}