using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xu.Common;

namespace Xu.Common
{
    /// <summary>
    /// appsettings.json操作类
    /// </summary>
    public class AppSettings
    {
        private static IConfiguration Configuration { get; set; }

        public AppSettings(string contentPath)
        {
            string Path = "appsettings.json";

            //如果你把配置文件 是 根据环境变量来分开了，可以这样写
            //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

            Configuration = new ConfigurationBuilder()
               .SetBasePath(contentPath)
               .Add(new JsonConfigurationSource { Path = Path, Optional = false, ReloadOnChange = true })//这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
               .Build();
        }

        public AppSettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        public static string App(params string[] sections)
        {
            try
            {
                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception) { }

            return "";
        }

        /// <summary>
        /// 递归获取配置信息数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<T> App<T>(params string[] sections)
        {
            List<T> list = new List<T>();
            // 引用 Microsoft.Extensions.Configuration.Binder 包
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }

        /// <summary>
        /// 根据路径  configuration["App:Name"];
        /// </summary>
        /// <param name="sectionsPath"></param>
        /// <returns></returns>
        public static string GetValue(string sectionsPath)
        {
            try
            {
                return Configuration[sectionsPath];
            }
            catch (Exception) { }

            return "";
        }
    }
}

#region 使用方式，多种方式均可使用

//1、按一定的层级路径，组成多个逗号隔开的一组参数
//Permissions.IsUseIds4 = AppSettings.app("Startup", "IdentityServer4", "Enabled").ObjToBool();
//RoutePrefix.Name = AppSettings.app("AppSettings", "SvcName").ObjToString();


//2、按照一定的层级路径，组成字符串数组

//Permissions.IsUseIds4 = AppSettings.app(new string[] { "Startup", "IdentityServer4", "Enabled" }).ObjToBool();
//RoutePrefix.Name = AppSettings.app(new string[] { "AppSettings", "SvcName" }).ObjToString();


//3、按照一定的层级路径，组成冒号隔开的字符串

//string PermissionServName = AppSettings.GetValue("ApiGateWay:PermissionServName");
//string PermissionServGroup = AppSettings.GetValue("ApiGateWay:PermissionServGroup");
//string PermissionServUrl = AppSettings.GetValue("ApiGateWay:PermissionServUrl");


//4、返回结果除了是字符串以外，也支持返回List泛型数组或对象
//List<MutiDBOperate> listdatabase = AppSettings.app<MutiDBOperate>("DBS")
//    .Where(i => i.Enabled).ToList();
//List<Urlobj> WhiteList = _cache.Cof_GetICaching<List<Urlobj>>("WhiteList", () => AppSettings.app<Urlobj>("WhiteList"), 10);


#endregion