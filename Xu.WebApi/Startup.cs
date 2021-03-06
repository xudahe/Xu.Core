using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Xu.Common;
using Xu.Extensions;
using Xu.IServices;
using Xu.Model;
using Xu.Tasks;

namespace Xu.WebApi
{
    public class Startup
    {
        private IServiceCollection _services;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        /// <summary>
        /// 应用程序运行时将服务添加到容器中
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>
        /// 权重：AddSingleton→AddTransient→AddScoped
        /// AddSingleton的生命周期：项目启动-项目关闭 相当于静态类  只会有一个
        /// AddScoped   的生命周期：请求开始-请求结束  在这次请求中获取的对象都是同一个
        /// AddTransient的生命周期：请求获取-（GC回收-主动释放） 每一次获取的对象都不是同一个
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Appsettings(Configuration));
            services.AddSingleton(new LogLock(Env.ContentRootPath)); //接口请求日志

            services.AddMemoryCacheSetup();
            services.AddRedisCacheSetup();
            services.AddSqlsugarSetup();
            services.AddDbSetup();
            services.AddAutoMapperSetup();
            services.AddCorsSetup();
            services.AddMiniProfilerSetup();
            services.AddSwaggerSetup();
            services.AddJobSetup();
            services.AddHttpContextSetup();
            services.AddAppConfigSetup();
            services.AddHttpApi();
            services.AddRedisInitMqSetup();
            //services.AddHstsSetup(); // 生产环境中使用
            //services.AddAntiforgerySetup(); //防止CSRF攻击

            Permissions.IsUseIds4 = Appsettings.App(new string[] { "Startup", "IdentityServer4", "Enabled" }).ToBoolReq();
            // 授权+认证 (jwt or ids4)
            services.AddAuthorizationSetup();
            if (Permissions.IsUseIds4)
            {
                services.AddAuthentication_Ids4Setup();
            }
            else
            {
                services.AddAuthentication_JWTSetup();
            }

            services.AddIpPolicyRateLimitSetup(Configuration);
            services.AddSignalR().AddNewtonsoftJsonProtocol();
            //配置可以同步请求读取流数据
            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                    .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            //services.AddRouting(options =>
            //{
            //    options.LowercaseUrls = true; //小写url的路由
            //});

            //启用控制器
            services.AddControllers(options =>
            {
                if (Appsettings.App(new string[] { "RSACryption", "Enabled" }).ToBoolReq())
                {
                    options.Filters.Add(typeof(DataDecryptFilter)); //数据解密过滤器
                }

                //全局XSS过滤器
                //options.Filters.Add(typeof(XSSFilterAttribute));
                //全局给post Action都开启了防止CSRF攻击,配合services.AddAntiforgerySetup()使用
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                // 全局异常过滤
                options.Filters.Add(typeof(GlobalExceptionsFilter));
                // 全局路由权限公约
                //o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
                // 全局路由前缀，统一修改路由
                options.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(RoutePrefix.Name)));
            })
            //全局配置Json序列化处理
            .AddNewtonsoftJson(options =>
            {
                //忽略循环引用，如果设置为Error，则遇到循环引用的时候报错
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //json中属性开头字母小写的驼峰命名
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //日期格式化
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //如果字段为null,该字段会依然返回到json中。比如"name":null
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            });
            //.ConfigureApiBehaviorOptions(options =>
            //{
            //    options.SuppressConsumesConstraintForFormFileParameters = true;
            //    options.SuppressInferBindingSourcesForParameters = true;
            //    options.SuppressModelStateInvalidFilter = true; //true：禁用自动 400 行为
            //    options.SuppressMapClientErrors = true;
            //    options.ClientErrorMapping[404].Link =
            //        "https://*/404";
            //});

            _services = services;
        }

        // 注意在Program.CreateHostBuilder，添加Autofac服务工厂
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MyContext myContext, ITasksQzSvc tasksQzSvc, ISchedulerCenter schedulerCenter, IHostApplicationLifetime lifetime)
        {
            // Ip限流,尽量放管道外层
            app.UseIpLimitMildd();
            // 记录请求与返回数据
            app.UseReuestResponseLog();
            // signalr
            app.UseSignalRSendMildd();
            // 记录ip请求
            app.UseIPLogMildd();
            // 查看注入的所有服务
            app.UseAllServicesMildd(_services);

            if (env.IsDevelopment())
            {
                // 在开发环境中，使用异常页面，这样可以暴露错误堆栈信息，所以不要放在生产环境。
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // 在非开发环境中，使用HTTP严格安全传输(or HSTS) 对于保护web安全是非常重要的。
                // 强制实施 HTTPS 在 ASP.NET Core，配合 app.UseHttpsRedirection
                //app.UseHsts(); // HSTS 中间件（UseHsts）用于向客户端发送 HTTP 严格传输安全协议（HSTS）标头
            }

            // 封装Swagger展示
            app.UseSwaggerMildd(() => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Xu.WebApi.index.html"));

            // ↓↓↓↓↓↓ 注意下边这些中间件的顺序，很重要 ↓↓↓↓↓↓

            // CORS跨域
            app.UseCors(Appsettings.App(new string[] { "Startup", "Cors", "PolicyName" }));
            // 重定向中间件，用于将 HTTP 请求重定向到 HTTPS
            //app.UseHttpsRedirection();
            // 默认使用wwwroot静态文件
            app.UseStaticFiles();
            // 使用cookie
            app.UseCookiePolicy();
            // 返回错误码
            app.UseStatusCodePages();
            // Routing
            app.UseRouting();
            // 这种自定义授权中间件，可以尝试，但不推荐
            // app.UseJwtTokenAuth();
            // 先开启认证--验证当前请求的用户，并设置HttpContext.User，当OAuth callbacks时，会中止执行下一个中间件。
            app.UseAuthentication();
            // 然后是授权中间件
            app.UseAuthorization();
            // 开启异常中间件，要放到最后
            app.UseExceptionHandlerMidd();
            // 性能分析
            app.UseMiniProfiler();
            // 用户访问记录
            app.UseRecordAccessLogsMildd();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<ChatHub>("/api/chatHub");
            });

            // 生成种子数据
            app.UseSeedDataMildd(myContext, Env.WebRootPath);
            // 开启QuartzNetJob调度服务
            app.UseQuartzJobMildd(tasksQzSvc, schedulerCenter);
            // 服务注册
            //app.UseConsulMildd(Configuration, lifetime);
        }
    }
}