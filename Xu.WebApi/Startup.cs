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
        /// log4net �ִ���
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
        /// Ȩ�أ�AddSingleton��AddTransient��AddScoped
        /// AddSingleton���������ڣ���Ŀ����-��Ŀ�ر� �൱�ھ�̬��  ֻ����һ��
        /// AddScoped   ���������ڣ�����ʼ-�������  ����������л�ȡ�Ķ�����ͬһ��
        /// AddTransient���������ڣ������ȡ-��GC����-�����ͷţ� ÿһ�λ�ȡ�Ķ��󶼲���ͬһ��
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<EFContext>(options => options.UseMySql(BaseDBConfig.ConnectionString));

            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();// ��Redis����ע�뵽�����У�����Controller�е��ã�����˵�£�������Լ�����Ŀ�����˸�����ʹ�õ���ģʽ
            services.AddSingleton(new Appsettings(Env.ContentRootPath));
            services.AddSingleton(new LogLock(Env.ContentRootPath)); //�ӿ�������־
            services.AddSingleton<ILogHelper, LogHelper>(); //log��־ע��

            services.AddMemoryCacheSetup();
            services.AddSqlsugarSetup();
            services.AddDbSetup(); //�Զ�����ʼ���ݿ�
            services.AddAutoMapperSetup();  //AutoMapper�Զ�ӳ��
            services.AddCorsSetup();  //���� CORS ����
            services.AddMiniProfilerSetup(); //�ӿ�ִ��ʱ�����
            services.AddSwaggerSetup();
            services.AddJobSetup();
            services.AddHttpContextSetup();
            services.AddAuthorizationSetup();

            services.AddSignalR().AddNewtonsoftJsonProtocol();

            //services.AddScoped<UseServiceDIAttribute>();

            services.AddControllers(o =>
            {
                // ȫ���쳣����
                o.Filters.Add(typeof(GlobalExceptionsFilter));
                // ȫ��·��Ȩ�޹�Լ
                //o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
                // ȫ��·��ǰ׺��ͳһ�޸�·��
                o.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(RoutePrefix.Name)));
            })
            //ȫ������Json���л�����
            .AddNewtonsoftJson(options =>
            {
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //��ʹ���շ���ʽ��key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //����ʱ���ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
        }

        // ע����CreateDefaultBuilder�У����Autofac���񹤳���AutoFac����ע��
        // ͨ��������ע������Ͳִ���ĳ��� dll ��ʵ������ע��
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

            //ע��Ҫͨ�����䴴�������
            //builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();
            builder.RegisterType<CacheAOP>();//����ֱ���滻����������
            builder.RegisterType<RedisCacheAOP>();//����ֱ���滻����������
            builder.RegisterType<LogAOP>();//��������ע��ڶ���
            builder.RegisterType<TranAOP>();

            #region ���нӿڲ�ķ���ע��

            //��ȡ��Ŀ����·������ע�⣬�����ʵ�����dll�ļ������ǽӿ� IService.dll ��ע��������Ȼ��Activatore
            try
            {
                #region AOP ���أ������Ҫ��ָ���Ĺ��ܣ�ֻ��Ҫ�� appsettigns.json ��Ӧ��Ӧ true ���С�

                var cacheType = new List<Type>();
                if (Appsettings.App(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).ToBoolReq())
                {
                    cacheType.Add(typeof(RedisCacheAOP)); //Redis��������
                }
                if (Appsettings.App(new string[] { "AppSettings", "MemoryCachingAOP", "Enabled" }).ToBoolReq())
                {
                    cacheType.Add(typeof(CacheAOP)); //memory��������
                }
                if (Appsettings.App(new string[] { "AppSettings", "TranAOP", "Enabled" }).ToBoolReq())
                {
                    cacheType.Add(typeof(TranAOP)); //Tran��������
                }
                if (Appsettings.App(new string[] { "AppSettings", "LogAOP", "Enabled" }).ToBoolReq())
                {
                    cacheType.Add(typeof(LogAOP)); //Log��־����
                }

                #endregion AOP ���أ������Ҫ��ָ���Ĺ��ܣ�ֻ��Ҫ�� appsettigns.json ��Ӧ��Ӧ true ���С�

                #region Service.dll ע�룬�ж�Ӧ�ӿ�

                var servicesDllFile = Path.Combine(basePath, "Xu.Services.dll"); //���ע�����ʵ����㣬���ǽӿڲ㣡���� IServices
                var assemblysServices = Assembly.LoadFrom(servicesDllFile);//ֱ�Ӳ��ü����ļ��ķ���

                builder.RegisterAssemblyTypes(assemblysServices) //ָ����ɨ������е�����ע��Ϊ�ṩ������ʵ�ֵĽӿڡ�
                          .AsImplementedInterfaces()
                          .InstancePerLifetimeScope()
                          .EnableInterfaceInterceptors()//����Autofac.Extras.DynamicProxy;
                                                        // �������ע������������ôд  InterceptedBy(typeof(XuCacheAOP), typeof(XuLogAOP));
                                                        // �����ʹ��Redis���棬����뿪�� redis ���񣬶˿ں��ҵ���6319�������һ��������Ч��������ʹ��memory���� CacheAOP
                          .InterceptedBy(cacheType.ToArray());//����������������б�����ע�ᡣ

                #endregion Service.dll ע�룬�ж�Ӧ�ӿ�

                #region Repository.dll ע�룬�ж�Ӧ�ӿ�

                var repositoryDllFile = Path.Combine(basePath, "Xu.Repository.dll");
                var assemblysRepository = Assembly.LoadFrom(repositoryDllFile); //���ע�����ʵ����㣬���ǽӿڲ㣡���� IRepository
                builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces(); //ָ����ɨ������е�����ע��Ϊ�ṩ������ʵ�ֵĽӿڡ�

                #endregion Repository.dll ע�룬�ж�Ӧ�ӿ�
            }
            catch (Exception ex)
            {
                log.Error("Repository.dll��service.dll ��ʧ����Ϊ��Ŀ�����ˣ�������Ҫ��F6���룬��F5���У����鲢������\n" + ex.Message);

                //throw new Exception("�����������Ϊ�����ˣ�������Ƿ�����ģʽ������bin�ļ����Ƿ����Repository.dll��service.dll ���������" + ex.Message + "\n" + ex.InnerException);
            }

            #endregion ���нӿڲ�ķ���ע��

            #region û�нӿڲ�ķ����ע��

            ////��Ϊû�нӿڲ㣬���Բ���ʵ�ֽ��ֻ���� Load ������
            ////ע�����ʹ��û�нӿڵķ��񣬲������ʹ�� AOP ���أ��ͱ�������Ϊ�鷽��
            ////var assemblysServicesNoInterfaces = Assembly.Load("Xu.Services");
            ////builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);

            #endregion û�нӿڲ�ķ����ע��

            #region û�нӿڵĵ����� class ע��

            ////ֻ��ע������е��鷽��
            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Love)))
            //    .EnableClassInterceptors()
            //    .InterceptedBy(typeof(XuLogAOP));

            #endregion û�нӿڵĵ����� class ע��
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // ��¼���еķ��ʼ�¼
            loggerFactory.AddLog4Net();
            // ��¼�����뷵������
            app.UseReuestResponseLog();
            // signalr
            app.UseSignalRSendMildd();
            // ��¼ip����
            app.UseIPLogMildd();

            #region Environment

            if (env.IsDevelopment())
            {
                // �ڿ��������У�ʹ���쳣ҳ�棬�������Ա�¶�����ջ��Ϣ�����Բ�Ҫ��������������
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // �ڷǿ��������У�ʹ��HTTP�ϸ�ȫ����(or HSTS) ���ڱ���web��ȫ�Ƿǳ���Ҫ�ġ�
                // ǿ��ʵʩ HTTPS �� ASP.NET Core����� app.UseHttpsRedirection
                //app.UseHsts();
            }

            #endregion Environment

            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //���ݰ汾���Ƶ��� ����չʾ
                var ApiName = Appsettings.App(new string[] { "Startup", "ApiName" });
                typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    //c.InjectOnCompleteJavaScript();
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });
                // ��swagger��ҳ�����ó������Զ����ҳ�棬�ǵ�����ַ�����д�������������.index.html
                c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Xu.WebApi.index.html");//���������MiniProfiler�������ܼ�ص�,����㲻��Ҫ��������ʱ��ע�͵�����Ӱ���֡�
                c.RoutePrefix = ""; //·�����ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�,ע��localhost:8001/swagger�Ƿ��ʲ����ģ�ȥlaunchSettings.json��launchUrlȥ����������뻻һ��·����ֱ��д���ּ��ɣ�����ֱ��дc.RoutePrefix = "doc";
            });

            #endregion Swagger

            // ������������ ע���±���Щ�м����˳�� ������������

            //CORS������һ��Ҫ����AutoFacǰ��
            //�� CORS �м����ӵ� web Ӧ�ó��������, ����������Ӧ�ÿ�������
            app.UseCors("LimitRequests");

            // ��תhttps
            //app.UseHttpsRedirection();
            // ʹ�þ�̬�ļ�
            app.UseStaticFiles();
            // ʹ��cookie
            app.UseCookiePolicy();
            // ���ش�����
            app.UseStatusCodePages();//�Ѵ����뷵��ǰ̨��������404
            // Routing ·���м��
            app.UseRouting();
            // �����Զ�����Ȩ�м�������Գ��ԣ������Ƽ�
            // app.UseJwtTokenAuth();
            // �ȿ�����֤
            app.UseAuthentication();
            // Ȼ������Ȩ�м��
            app.UseAuthorization();

            // �����쳣�м����Ҫ�ŵ����
            //app.UseExceptionHandlerMidd();

            app.UseMiniProfiler();

            // ��·�м��������Controller·��
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                //��Ϊ��ǰ��˷����ˣ�����ʹ�õ��Ǵ���ģʽ����������㲻��/api/xxx���������Ļ�������ֿ������⣬�Ͼ���������ҵ�controller��·�ɣ������Լ������·��
                endpoints.MapHub<ChatHub>("/api/chatHub");
            });
        }
    }
}