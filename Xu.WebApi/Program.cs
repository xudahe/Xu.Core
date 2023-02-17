// 以下为asp.net 6.0的写法
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Text;
using Xu.Common;
using Xu.Extensions;
using Xu.Extensions.Apollo;
using Xu.Extensions.Middlewares;
using Xu.IServices;
using Xu.Model;
using Xu.Tasks;
using Xu.WebApi;

var builder = WebApplication.CreateBuilder(args);

// 1、配置host与容器
builder.Host
.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(builder =>
{
    //添加Autofac服务工厂
    builder.RegisterModule(new AutofacModuleRegister());
    builder.RegisterModule<AutofacPropertityModuleReg>();
})
.ConfigureLogging((hostingContext, builder) =>
{
    //过滤掉系统默认的一些日志
    builder.AddFilter("System", LogLevel.Error);
    builder.AddFilter("Microsoft", LogLevel.Error);
    // 统一设置
    builder.SetMinimumLevel(LogLevel.Error);
    //可配置文件，不带参数：表示log4net.config的配置文件就在应用程序根目录下，也可以指定配置文件的路径
    builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
})
.ConfigureAppConfiguration((hostingContext, config) =>
{
    //config.Sources.Clear();//清除已有的所有配置（包括appsettings.json配置）
    //config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
    config.AddConfigurationApollo("appsettings.apollo.json");    //接入Apollo配置中心
});

// 2、配置服务
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddSingleton(new LogLock(builder.Environment.ContentRootPath));//接口请求日志
//builder.Services.AddUiFilesZipSetup(builder.Environment);

Permissions.IsUseIds4 = AppSettings.App(new string[] { "Startup", "IdentityServer4", "Enabled" }).ToBoolReq();
Permissions.IsUseAuthing = AppSettings.App(new string[] { "Startup", "Authing", "Enabled" }).ToBoolReq();
RoutePrefix.Name = AppSettings.App(new string[] { "AppSettings", "SvcName" }).ObjToString();

// 确保从ids4认证中心返回的ClaimType不被更改，不使用Map映射
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddMemoryCacheSetup();
builder.Services.AddRedisCacheSetup();
builder.Services.AddSqlsugarSetup();
builder.Services.AddDbSetup();
builder.Services.AddAutoMapperSetup();
builder.Services.AddCorsSetup();
builder.Services.AddMiniProfilerSetup();
builder.Services.AddSwaggerSetup();
builder.Services.AddJobSetup();
builder.Services.AddHttpContextSetup();
// builder.Services.AddAppConfigSetup();
builder.Services.AddAppTableConfigSetup(builder.Environment);//表格打印配置
builder.Services.AddHttpApi();
//builder.Services.AddHstsSetup(); // 生产环境中使用
//builder.Services.AddAntiforgerySetup(); //防止CSRF攻击
builder.Services.AddRedisInitMqSetup();
builder.Services.AddRabbitMQSetup();
builder.Services.AddKafkaSetup(builder.Configuration);
builder.Services.AddEventBusSetup();
builder.Services.AddNacosSetup(builder.Configuration);

// 授权+认证 (jwt or ids4)
builder.Services.AddAuthorizationSetup();
if (Permissions.IsUseIds4 || Permissions.IsUseAuthing)
{
    if (Permissions.IsUseIds4) builder.Services.AddAuthentication_Ids4Setup();
    else if (Permissions.IsUseAuthing) builder.Services.AddAuthentication_AuthingSetup();
}
else
{
    builder.Services.AddAuthentication_JWTSetup();
}

builder.Services.AddIpPolicyRateLimitSetup(builder.Configuration);

builder.Services.AddSignalR(hubOptions =>
    {
        //注意：建议 服务端clientTimeoutInterval 的值是 客户端keepAliveIntervalInmillisecods 的两倍，从而保证不进服务器端的 OnDisconnectedAsync 回调，即不掉线
        hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(30); //服务器端配置30s没有收到客户端发送的消息，则认为客户端已经掉线
    }).AddNewtonsoftJsonProtocol();

builder.Services.AddScoped<UseServiceDIAttribute>();

//配置可以同步请求读取流数据
builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);
//builder.Services.AddRouting(options =>
//{
//    options.LowercaseUrls = true; //小写url的路由
//});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpPollySetup();

builder.Services.AddControllers(options => //启用控制器
{
    if (AppSettings.App(new string[] { "RSACryption", "Enabled" }).ToBoolReq())
    {
        options.Filters.Add(typeof(DataDecryptFilter)); //数据解密过滤器
    }

    //全局XSS过滤器
    //options.Filters.Add(typeof(XSSFilterAttribute));

    //全局给post Action都开启了防止CSRF攻击,配合services.AddAntiforgerySetup()使用
    //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

    //全局配置防止SQL注入过滤（或者在控制器方法上添加[AntiSqlInjectFilter]）
    //options.Filters.Add(typeof(AntiSqlInjectFilter));

    // 全局异常过滤
    options.Filters.Add(typeof(GlobalExceptionsFilter));

    // 全局路由权限公约
    //o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());

    // 全局路由前缀，统一修改路由
    options.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(RoutePrefix.Name)));
})
.AddNewtonsoftJson(options => //全局配置Json序列化处理
{
    //忽略循环引用，如果设置为Error，则遇到循环引用的时候报错
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

    //json中属性开头字母小写的驼峰命名
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

    //日期格式化
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

    //空值的处理
    //Ignore 序列化和反序列化对象时忽略空值
    //Include 序列化和反序列化对象时包含空值
    options.SerializerSettings.NullValueHandling = NullValueHandling.Include; //比如"name":null

    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
})
.AddFluentValidation(config =>
{
    //程序集方式添加验证
    config.RegisterValidatorsFromAssemblyContaining(typeof(UserRegisterVoValidator));
    //是否与MvcValidation共存
    config.DisableDataAnnotationsValidation = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

//支持编码大全 例如:支持 System.Text.Encoding.GetEncoding("GB2312")  System.Text.Encoding.GetEncoding("GB18030")
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

#region //配置启动地址

//builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(5000, opts => opts.Protocols = HttpProtocols.Http1));
//builder.WebHost.UseUrls("http://localhost:1081;https://localhost:1082");

//如果用IIS部署 需要注释UseKestrel
builder.WebHost.UseKestrel((host, options) =>
{
   options.ListenAnyIP(1081);
   options.ListenAnyIP(1082);
   //options.Listen(IPAddress.Loopback, 5000);
   //options.Listen(IPAddress.Loopback, 5001);
   //options.ListenLocalhost(5004, opts => opts.UseHttps());
});

#endregion //配置启动地址

// 3、配置中间件
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // 在开发环境中，使用异常页面，这样可以暴露错误堆栈信息，所以不要放在生产环境。
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // 在非开发环境中，使用HTTP严格安全传输(or HSTS) 对于保护web安全是非常重要的。
    // 强制实施 HTTPS 在 ASP.NET Core，需要配合 app.UseHttpsRedirection()与services.AddHstsSetup()
    //app.UseHsts(); // HSTS 中间件（UseHsts）用于向客户端发送 HTTP 严格传输安全协议（HSTS）标头
}

app.UseIpLimitMiddle();// Ip限流,尽量放管道外层
app.UseRequRespLogMiddle();// 记录请求与返回数据
app.UseRecordAccessLogsMiddle();// 用户访问记录(必须放到外层，不然如果遇到异常，会报错，因为不能返回流)
app.UseSignalRSendMiddle();// 注册signalr消息推送中间件
app.UseIpLogMiddle();// 记录ip请求
app.UseAllServicesMiddle(builder.Services);   // 查看注入的所有服务

// 自定义Swagger权限拦截中间件，放到Swagger中间件之前
app.UseSession();
app.UseSwaggerAuthorized();
// 封装Swagger展示
app.UseSwaggerMiddle(() => Assembly.GetExecutingAssembly().GetManifestResourceStream("Xu.WebApi.index.html"));

// ↓↓↓↓↓↓ 注意下边这些中间件的顺序，很重要 ↓↓↓↓↓↓
app.UseCors(AppSettings.App(new string[] { "Startup", "Cors", "PolicyName" }));   // CORS跨域
//app.UseHttpsRedirection();  // 重定向中间件，用于将 HTTP 请求重定向到 HTTPS

// 使用静态文件
DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(defaultFilesOptions);

app.UseStaticFiles();// 默认使用wwwroot静态文件
app.UseCookiePolicy();  // 使用cookie
app.UseStatusCodePages();     // 返回错误码
app.UseRouting();  // Routing

if (builder.Configuration.GetValue<bool>("AppSettings:UseLoadTest"))
{
    app.UseMiddleware<ByPassAuthMiddle>();
}

// 先开启认证--验证当前请求的用户，并设置HttpContext.User，当OAuth callbacks时，会中止执行下一个中间件。
app.UseAuthentication();
// 然后是授权中间件
app.UseAuthorization();
// 开启性能分析
app.UseMiniProfilerMiddle();
// 开启异常中间件，要放到最后
//app.UseExceptionHandlerMidd();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
         name: "default",
         pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapHub<ChatHub>("/api/chatHub");
});

var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var myContext = scope.ServiceProvider.GetRequiredService<MyContext>();
var tasksQzServices = scope.ServiceProvider.GetRequiredService<ITasksQzSvc>();
var schedulerCenter = scope.ServiceProvider.GetRequiredService<ISchedulerCenter>();
var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
app.UseSeedDataMilddle(myContext, builder.Environment.WebRootPath);  // 生成种子数据
app.UseQuartzJobMiddle(tasksQzServices, schedulerCenter);  // 开启QuartzNetJob调度服务
app.UseConsulMiddle(builder.Configuration, lifetime);  // 服务注册
app.ConfigureEventBus();    // 事件总线，订阅服务

// 4、运行
app.Run();