using AspNetCoreRateLimit;
using log4net;
using Microsoft.AspNetCore.Builder;
using System;
using Xu.Common;

namespace Xu.Extensions.Middlewares
{
    /// <summary>
    /// ip 限流
    /// </summary>
    public static class IpLimitMiddle
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IpLimitMiddle));

        public static void UseIpLimitMiddle(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppSettings.App("Middleware", "IpRateLimit", "Enabled").ToBoolReq())
                {
                    app.UseIpRateLimiting();
                }
            }
            catch (Exception e)
            {
                log.Error($"Error occured limiting ip rate.\n{e.Message}");
                throw;
            }
        }
    }
}