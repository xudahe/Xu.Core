using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Xu.Common
{
    /// <summary>
    /// Serilog 记录日志
    /// </summary>
    public class SerilogServer
    {
        private static string _contentRoot = string.Empty;

        public SerilogServer(string contentPath)
        {
            _contentRoot = contentPath;
        }

        /// <summary>
        /// 记录日常日志
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dataParas"></param>
        /// <param name="IsHeader"></param>
        /// <param name="defaultFolder"></param>
        /// <param name="isJudgeJsonFormat"></param>
        public static void WriteLog(string filename, string[] dataParas, bool IsHeader = true, string defaultFolder = "", bool isJudgeJsonFormat = false)
        {
            var path = Path.Combine(_contentRoot, "Log", DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(path))//如果路径不存在
            {
                Directory.CreateDirectory(path);//创建一个路径的文件夹
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                //.WriteTo.File(Path.Combine("Log", defaultFolder, $"{filename}.log"),
                .WriteTo.File(Path.Combine(path, $@"{filename}.log"),
                rollingInterval: RollingInterval.Infinite,
                outputTemplate: "{Message}{NewLine}{Exception}")

                // 将日志托送到远程ES
                // docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" -e ES_JAVA_OPTS="-Xms256m -Xmx256m" -d --name ES01 elasticsearch:7.2.0
                //.Enrich.FromLogContext()
                //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://x.xxx.xx.xx:9200/"))
                //{
                //    AutoRegisterTemplate = true,
                //})

                .CreateLogger();

            var now = DateTime.Now;
            string logContent = String.Join("\r\n", dataParas);
            var isJsonFormat = true;
            if (isJudgeJsonFormat)
            {
                var judCont = logContent.Substring(0, logContent.LastIndexOf(","));
                isJsonFormat = JsonHelper.IsJson(judCont);
            }

            if (isJsonFormat)
            {
                if (IsHeader)
                {
                    logContent = (
                       "--------------------------------\r\n" +
                       DateTime.Now + "|\r\n" +
                       String.Join("\r\n", dataParas) + "\r\n"
                       );
                }
                // 展示elk支持输出4种日志级别
                Log.Information(logContent);
                //Log.Warning(logContent);
                //Log.Error(logContent);
                //Log.Debug(logContent);
            }
            else
            {
                Console.WriteLine("【JSON格式异常：】" + logContent + now.ObjToString());
            }
            Log.CloseAndFlush();
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void WriteErrorLog(string filename, string message, Exception ex)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .WriteTo.File(Path.Combine($"log/Error/{filename}/", ".txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Error(ex, message);
            Log.CloseAndFlush();
        }
    }
}