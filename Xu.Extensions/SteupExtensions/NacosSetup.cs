﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nacos.V2.DependencyInjection;
using System;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// Nacos注册中心配置
    /// </summary>
    public static class NacosSetup
    {
        public static void AddNacosSetup(this IServiceCollection services, IConfiguration Configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // 在实际生产工作中 本地开发是不需要注册nacos的 所以根据环境变量去判断
            // 比如 开发环境 dev  测试环境 test  生产 prod  只有这几种环境变量的时候才需要去注册nacos
            if (AppSettings.App(new string[] { "Startup", "Nacos", "Enabled" }).ToBoolReq())
            {
                // 从当前配置取文件去注册naocs
                services.AddNacosV2Config(x =>
                {
                    x.ServerAddresses = JsonConfigSettings.NacosServerAddresses;
                    x.EndPoint = "";
                    x.Namespace = JsonConfigSettings.NacosNamespace;
                    x.DefaultTimeOut = JsonConfigSettings.NacosDefaultTimeOut;
                    x.ListenInterval = JsonConfigSettings.ListenInterval;
                    // swich to use http or rpc
                    x.ConfigUseRpc = false;
                });
                services.AddNacosV2Naming(x =>
                {
                    x.ServerAddresses = JsonConfigSettings.NacosServerAddresses;
                    x.EndPoint = "";
                    x.Namespace = JsonConfigSettings.NacosNamespace;
                    x.DefaultTimeOut = JsonConfigSettings.NacosDefaultTimeOut;
                    x.ListenInterval = JsonConfigSettings.ListenInterval;
                    // swich to use http or rpc
                    x.NamingUseRpc = false;
                });
                services.AddHostedService<NacosListenNamingTask>(); //增加服务注入，删除事件
                // 监听nacos中的配置中心 如果有新配置变更 执行相关逻辑
                services.AddHostedService<NacosListenConfigurationTask>();//增加配置文件监听事件
            }

            services.AddSingleton(Configuration);
        }
    }
}