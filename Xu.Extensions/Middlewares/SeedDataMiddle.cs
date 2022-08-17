using log4net;
using Microsoft.AspNetCore.Builder;
using System;
using Xu.Common;
using Xu.Model;

namespace Xu.Extensions.Middlewares
{
    /// <summary>
    /// 生成种子数据中间件服务
    /// </summary>
    public static class SeedDataMiddle
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SeedDataMiddle));

        public static void UseSeedDataMilddle(this IApplicationBuilder app, MyContext myContext, string webRootPath)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (Appsettings.App("AppSettings", "SeedDBEnabled").ToBoolReq() || Appsettings.App("AppSettings", "SeedDBDataEnabled").ToBoolReq())
                {
                    DBSeed.SeedAsync(myContext, webRootPath).Wait();
                }
            }
            catch (Exception e)
            {
                log.Error($"Error occured seeding the Database.\n{e.Message}");
                throw;
            }
        }
    }
}