using Microsoft.Extensions.Logging;
using SqlSugar;
using Xu.Common;

namespace Xu.WebApi
{
    public static class Log4netExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string log4NetConfigFile)
        {
            factory.AddProvider(new Log4NetProvider(log4NetConfigFile));
            return factory;
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory)
        {
            if (Appsettings.App("Middleware", "RecordAllLogs", "Enabled").ObjToBool())
            {
                factory.AddProvider(new Log4NetProvider("Log4net.config"));
            }
            return factory;
        }
    }
}