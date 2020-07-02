using Autofac;
using Autofac.Extras.DynamicProxy;
using log4net;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xu.Common;
using static Xu.WebApi.CustomApiVersion;

namespace Xu.WebApi
{
    public class Startup
    {
        /// <summary>
        /// log4net 仓储库
        /// </summary>
        public static ILoggerRepository Repository { get; set; }

        private static readonly ILog log = LogManager.GetLogger(typeof(GlobalExceptionsFilter));

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
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
            //services.AddDbContext<EFContext>(options => options.UseMySql(BaseDBConfig.ConnectionString));

            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();// 将Redis服务注入到容器中，并在Controller中调用，这里说下，如果是自己的项目，个人更建议使用单例模式
            services.AddSingleton(new Appsettings(Env.ContentRootPath));
            services.AddSingleton(new LogLock(Env.ContentRootPath)); //接口请求日志
            services.AddSingleton<ILogHelper, LogHelper>(); //log日志注入

            services.AddMemoryCacheSetup();
            services.AddSqlsugarSetup();
            services.AddDbSetup(); //自动化初始数据库
            services.AddAutoMapperSetup();  //AutoMapper自动映射
            services.AddCorsSetup();  //配置 CORS 跨域
            services.AddMiniProfilerSetup(); //接口执行时间分析
            services.AddSwaggerSetup();
            services.AddJobSetup();
            services.AddHttpContextSetup();
            services.AddAuthorizationSetup();

            services.AddSignalR().AddNewtonsoftJsonProtocol();

            //services.AddScoped<UseServiceDIAttribute>();

            services.AddControllers(o =>
            {
                // 全局异常过滤
                o.Filters.Add(typeof(GlobalExceptionsFilter));
                // 全局路由权限公约
                //o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
                // 全局路由前缀，统一修改路由
                o.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(RoutePrefix.Name)));
            })
            //全局配置Json序列化处理
            .AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
        }

        // 注意在CreateDefaultBuilder中，添加Autofac服务工厂，AutoFac容器注入
        // 通过反射来注入服务层和仓储层的程序集 dll 来实现批量注入
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

            //注册要通过反射创建的组件
            //builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();
            builder.RegisterType<CacheAOP>();//可以直接替换其他拦截器
            builder.RegisterType<RedisCacheAOP>();//可以直接替换其他拦截器
            builder.RegisterType<LogAOP>();//这样可以注入第二个
            builder.RegisterType<TranAOP>();

            #region 带有接口层的服务注入

            //获取项目绝对路径，请注意，这个是实现类的dll文件，不是接口 IService.dll ，注入容器当然是Activatore
            try
            {
                #region AOP 开关，如果想要打开指定的功能，只需要在 appsettigns.json 对应对应 true 就行。

                var cacheType = new List<Type>();
                if (Appsettings.App(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).ToBoolReq())
                {
                    cacheType.Add(typeof(RedisCacheAOP)); //Redis缓存切面
                }
                if (Appsettings.App(new string[] { "AppSettings", "MemoryCachingAOP", "Enabled" }).ToBoolReq())
                {
                    cacheType.Add(typeof(CacheAOP)); //memory缓存切面
                }
                if (Appsettings.App(new string[] { "AppSettings", "TranAOP", "Enabled" }).ToBoolReq())
                {
                    cacheType.Add(typeof(TranAOP)); //Tran事务切面
                }
                if (Appsettings.App(new string[] { "AppSettings", "LogAOP", "Enabled" }).ToBoolReq())
                {
                    cacheType.Add(typeof(LogAOP)); //Log日志切面
                }

                #endregion AOP 开关，如果想要打开指定的功能，只需要在 appsettigns.json 对应对应 true 就行。

                #region Service.dll 注入，有对应接口

                var servicesDllFile = Path.Combine(basePath, "Xu.Services.dll"); //这个注入的是实现类层，不是接口层！不是 IServices
                var assemblysServices = Assembly.LoadFrom(servicesDllFile);//直接采用加载文件的方法

                builder.RegisterAssemblyTypes(assemblysServices) //指定已扫描程序集中的类型注册为提供所有其实现的接口。
                          .AsImplementedInterfaces()
                          .InstancePerLifetimeScope()
                          .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                                                        // 如果你想注入两个，就这么写  InterceptedBy(typeof(XuCacheAOP), typeof(XuLogAOP));
                                                        // 如果想使用Redis缓存，请必须开启 redis 服务，端口号我的是6319，如果不一样还是无效，否则请使用memory缓存 CacheAOP
                          .InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。

                #endregion Service.dll 注入，有对应接口

                #region Repository.dll 注入，有对应接口

                var repositoryDllFile = Path.Combine(basePath, "Xu.Repository.dll");
                var assemblysRepository = Assembly.LoadFrom(repositoryDllFile); //这个注入的是实现类层，不是接口层！不是 IRepository
                builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces(); //指定已扫描程序集中的类型注册为提供所有其实现的接口。

                #endregion Repository.dll 注入，有对应接口
            }
            catch (Exception ex)
            {
                log.Error("Repository.dll和service.dll 丢失，因为项目解耦了，所以需要先F6编译，再F5运行，请检查并拷贝。\n" + ex.Message);

                //throw new Exception("※※★※※因为解耦了，如果你是发布的模式，请检查bin文件夹是否存在Repository.dll和service.dll ※※★※※" + ex.Message + "\n" + ex.InnerException);
            }

            #endregion 带有接口层的服务注入

            #region 没有接口层的服务层注入

            ////因为没有接口层，所以不能实现解耦，只能用 Load 方法。
            ////注意如果使用没有接口的服务，并想对其使用 AOP 拦截，就必须设置为虚方法
            ////var assemblysServicesNoInterfaces = Assembly.Load("Xu.Services");
            ////builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);

            #endregion 没有接口层的服务层注入

            #region 没有接口的单独类 class 注入

            ////只能注入该类中的虚方法
            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Love)))
            //    .EnableClassInterceptors()
            //    .InterceptedBy(typeof(XuLogAOP));

            #endregion 没有接口的单独类 class 注入
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // 记录所有的访问记录
            loggerFactory.AddLog4Net();
            // 记录请求与返回数据
            app.UseReuestResponseLog();
            // signalr
            app.UseSignalRSendMildd();
            // 记录ip请求
            app.UseIPLogMildd();

            #region Environment

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
                //app.UseHsts();
            }

            #endregion Environment

            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //根据版本名称倒序 遍历展示
                var ApiName = Appsettings.App(new string[] { "Startup", "ApiName" });
                typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    //c.InjectOnCompleteJavaScript();
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });
                // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：解决方案名.index.html
                c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Xu.WebApi.index.html");//这里是配合MiniProfiler进行性能监控的,如果你不需要，可以暂时先注释掉，不影响大局。
                c.RoutePrefix = ""; //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
            });

            #endregion Swagger

            // ↓↓↓↓↓↓ 注意下边这些中间件的顺序 ↓↓↓↓↓↓

            //CORS的配置一定要放在AutoFac前面
            //将 CORS 中间件添加到 web 应用程序管线中, 以允许整个应用跨域请求。
            app.UseCors("LimitRequests");

            // 跳转https
            //app.UseHttpsRedirection();
            // 使用静态文件
            app.UseStaticFiles();
            // 使用cookie
            app.UseCookiePolicy();
            // 返回错误码
            app.UseStatusCodePages();//把错误码返回前台，比如是404
            // Routing 路由中间件
            app.UseRouting();
            // 这种自定义授权中间件，可以尝试，但不推荐
            // app.UseJwtTokenAuth();
            // 先开启认证
            app.UseAuthentication();
            // 然后是授权中间件
            app.UseAuthorization();

            // 开启异常中间件，要放到最后
            //app.UseExceptionHandlerMidd();

            app.UseMiniProfiler();

            // 短路中间件，配置Controller路由
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                //因为我前后端分离了，而且使用的是代理模式，所以如果你不用/api/xxx的这个规则的话，会出现跨域问题，毕竟这个不是我的controller的路由，而且自己定义的路由
                endpoints.MapHub<ChatHub>("/api/chatHub");
            });
        }
    }
}