using System;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar.Extensions;

namespace Xu.Extensions.HostedService;

/// <summary>
/// Consul 注册服务, 它的核心功能就是服务注册和发现
/// </summary>
/// <remarks>
/// Consul 客户端通过将自己注册到 Consul 服务器集群，然后等待调用方去发现服务，实现代理转发到真正的业务系统，还可以基于服务发现做负载均衡，
/// 甚至可以在客户端请求到底服务之前进行拦截，做一些基础性的工作，比如身份验证、限流、熔断等等业务系统的前瞻性工作。
/// </remarks>
public class ConsulHostedService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<ConsulHostedService> _logger;

    public ConsulHostedService(IConfiguration configuration, IHostApplicationLifetime hostApplicationLifetime, ILogger<ConsulHostedService> logger)
    {
        _configuration = configuration;
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // _logger.LogInformation("Start Consul Service!");
        await DoWork();
    }

    public async Task DoWork()
    {
        if (_configuration["Middleware:Consul:Enabled"].ObjToBool())
        {
            var consulClient = new ConsulClient(c =>
            {
                //consul地址
                c.Address = new Uri(_configuration["ConsulSetting:ConsulAddress"]);
            });

            var registration = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),//服务实例唯一标识
                Name = _configuration["ConsulSetting:ServiceName"],//服务名
                Address = _configuration["ConsulSetting:ServiceIP"], //服务IP
                Port = int.Parse(_configuration["ConsulSetting:ServicePort"]),//服务端口
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔
                    HTTP = $"http://{_configuration["ConsulSetting:ServiceIP"]}:{_configuration["ConsulSetting:ServicePort"]}{_configuration["ConsulSetting:ServiceHealthCheck"]}",//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)//超时时间
                }
            };

            //服务注册
            await consulClient.Agent.ServiceRegister(registration);

            //应用程序终止时，取消注册
            _hostApplicationLifetime.ApplicationStopping.Register(async () =>
            {
                await consulClient.Agent.ServiceDeregister(registration.ID);
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // _logger.LogInformation("Stop Consul Service!");
        return Task.CompletedTask;
    }
}