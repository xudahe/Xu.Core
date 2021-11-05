using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using Xu.Extensions.Apollo;

namespace Xu.WebApi
{
    public class Program
    {
        /// <summary>
        /// Main方法负责初始化Web主机，调用Startup和执行应用程序
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //初始化默认主机Builder
            Host.CreateDefaultBuilder(args)
             .UseServiceProviderFactory(new AutofacServiceProviderFactory())
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder
                 .ConfigureKestrel(serverOptions =>
                 {
                     serverOptions.AllowSynchronousIO = true; //启用同步 IO
                     serverOptions.Limits.MaxRequestBodySize = int.MaxValue;//限制请求长度
                 })
                 .UseStartup<Startup>() //调用Startup.cs类下的Configure 和 ConfigureServices
                 .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     //config.Sources.Clear(); //清除已有的所有配置（包括appsettings.json配置）
                     //config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                     //接入Apollo配置中心
                     config.AddConfigurationApollo("appsettings.apollo.json");
                 })
                 .UseUrls("http://*:1081", "http://*:1082")
                 .ConfigureLogging((hostingContext, builder) =>
                 {
                     //过滤掉系统默认的一些日志
                     builder.AddFilter("System", LogLevel.Error);
                     builder.AddFilter("Microsoft", LogLevel.Error);
                     builder.AddFilter("Xu.Extensions.ApiResponseHandler", LogLevel.Error);

                     //可配置文件，不带参数：表示log4net.config的配置文件就在应用程序根目录下，也可以指定配置文件的路径
                     builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
                 });
             })
             // 生成承载 web 应用程序的 Microsoft.AspNetCore.Hosting.IWebHost。Build是WebHostBuilder最终的目的，将返回一个构造的WebHost，最终生成宿主。
             .Build()
             // 运行 web 应用程序并阻止调用线程, 直到主机关闭。
             // 创建完 WebHost 之后，便调用它的 Run 方法，而 Run 方法会去调用 WebHost 的 StartAsync 方法
             // 将Initialize方法创建的Application管道传入以供处理消息
             // 执行HostedServiceExecutor.StartAsync方法
             // ※※※※ 有异常，查看 Log 文件夹下的异常日志 ※※※※
             .Run();
        }
    }
}