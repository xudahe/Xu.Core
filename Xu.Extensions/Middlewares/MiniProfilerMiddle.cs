using log4net;
using Microsoft.AspNetCore.Builder;
using System;
using Xu.Common;

namespace Xu.Extensions.Middlewares
{
    /// <summary>
    /// MiniProfiler性能分析
    /// </summary>
    public static class MiniProfilerMiddle
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MiniProfilerMiddle));

        public static void UseMiniProfilerMiddle(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppSettings.App("Startup", "MiniProfiler", "Enabled").ToBoolReq())
                {
                    // 性能分析
                    app.UseMiniProfiler();
                }
            }
            catch (Exception e)
            {
                log.Error($"An error was reported when starting the MiniProfilerMildd.\n{e.Message}");
                throw;
            }
        }
    }
}