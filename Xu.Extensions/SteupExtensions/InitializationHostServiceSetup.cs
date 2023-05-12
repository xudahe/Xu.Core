using Microsoft.Extensions.DependencyInjection;
using System;
using Xu.Extensions.HostedService;

namespace Xu.Extensions;

public static class InitializationHostServiceSetup
{
    /// <summary>
    /// 应用初始化服务注入
    /// </summary>
    /// <param name="services"></param>
    public static void AddInitializationHostServiceSetup(this IServiceCollection services)
    {
        if (services is null)
        {
            ArgumentNullException.ThrowIfNull(nameof(services));
        }
        services.AddHostedService<SeedDataHostedService>();  // 生成种子数据
        services.AddHostedService<QuartzJobHostedService>(); // 开启QuartzNetJob调度服务
        services.AddHostedService<ConsulHostedService>();
        services.AddHostedService<EventBusHostedService>();
    }
}