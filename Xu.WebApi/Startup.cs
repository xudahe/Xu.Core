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
        /// Ӧ�ó�������ʱ��������ӵ�������
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
            // ����code�����������в�һ��,�Դ������˷�װ,����鿴�Ҳ� Extensions �ļ���.
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>(); //��Redis����ע�뵽�����У�����Controller�е���
            services.AddSingleton(new Appsettings(Configuration));
            services.AddSingleton(new LogLock(Env.ContentRootPath)); //�ӿ�������־

            Permissions.IsUseIds4 = Appsettings.App(new string[] { "Startup", "IdentityServer4", "Enabled" }).ToBoolReq();

            services.AddMemoryCacheSetup();
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
            //services.AddHstsSetup(); // ����������ʹ��
            //services.AddAntiforgerySetup(); //��ֹCSRF����
            if (Permissions.IsUseIds4)
            {
                services.AddAuthorization_Ids4Setup();
            }
            else
            {
                services.AddAuthorizationSetup();
            }
            services.AddIpPolicyRateLimitSetup(Configuration);
            services.AddSignalR().AddNewtonsoftJsonProtocol();
            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                    .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            //services.AddRouting(options =>
            //{
            //    options.LowercaseUrls = true; //Сдurl��·��
            //});

            services.AddControllers(options =>
            {
                //ȫ��XSS������
                //options.Filters.Add(typeof(XSSFilterAttribute));
                //ȫ�ָ�post Action�������˷�ֹCSRF����,���services.AddAntiforgerySetup()ʹ��
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                // ȫ���쳣����
                options.Filters.Add(typeof(GlobalExceptionsFilter));
                // ȫ��·��Ȩ�޹�Լ
                //o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
                // ȫ��·��ǰ׺��ͳһ�޸�·��
                options.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(RoutePrefix.Name)));
            })
            //ȫ������Json���л�����
            .AddNewtonsoftJson(options =>
            {
                //����ѭ�����ã��������ΪError��������ѭ�����õ�ʱ�򱨴�
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //json�����Կ�ͷ��ĸСд���շ�����
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //���ڸ�ʽ��
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //����ֶ�Ϊnull,���ֶλ���Ȼ���ص�json�С�����"name":null
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            });

            _services = services;
        }

        // ע����Program.CreateHostBuilder�����Autofac���񹤳�
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MyContext myContext, ITasksQzSvc tasksQzSvc, ISchedulerCenter schedulerCenter, IHostApplicationLifetime lifetime)
        {
            // Ip����,�����Źܵ����
            app.UseIpLimitMildd();
            // ��¼�����뷵������
            app.UseReuestResponseLog();
            // signalr
            app.UseSignalRSendMildd();
            // ��¼ip����
            app.UseIPLogMildd();
            // �鿴ע������з���
            app.UseAllServicesMildd(_services);

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
                //app.UseHsts(); // HSTS �м����UseHsts��������ͻ��˷��� HTTP �ϸ��䰲ȫЭ�飨HSTS����ͷ
            }

            // ��װSwaggerչʾ
            app.UseSwaggerMildd(() => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Xu.WebApi.index.html"));

            // ������������ ע���±���Щ�м����˳�򣬺���Ҫ ������������

            // CORS����
            app.UseCors("LimitRequests");
            // �ض����м�������ڽ� HTTP �����ض��� HTTPS
            //app.UseHttpsRedirection();
            // ʹ�þ�̬�ļ�
            app.UseStaticFiles();
            // ʹ��cookie
            app.UseCookiePolicy();
            // ���ش�����
            app.UseStatusCodePages();
            // Routing
            app.UseRouting();
            // �����Զ�����Ȩ�м�������Գ��ԣ������Ƽ�
            // app.UseJwtTokenAuth();
            // �ȿ�����֤--��֤��ǰ������û���������HttpContext.User����OAuth callbacksʱ������ִֹ����һ���м����
            app.UseAuthentication();
            // Ȼ������Ȩ�м��
            app.UseAuthorization();
            // �����쳣�м����Ҫ�ŵ����
            app.UseExceptionHandlerMidd();
            // ���ܷ���
            app.UseMiniProfiler();
            // �û����ʼ�¼
            app.UseRecordAccessLogsMildd();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<ChatHub>("/api/chatHub");
            });

            // ������������
            app.UseSeedDataMildd(myContext, Env.WebRootPath);
            // ����QuartzNetJob���ȷ���
            app.UseQuartzJobMildd(tasksQzSvc, schedulerCenter);
            // ����ע��
            //app.UseConsulMildd(Configuration, lifetime);
        }
    }
}