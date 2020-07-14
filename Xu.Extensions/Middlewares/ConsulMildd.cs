using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// Consul 注册服务, 它的核心功能就是服务注册和发现
    /// </summary>
    /// <remarks>
    /// Consul 客户端通过将自己注册到 Consul 服务器集群，然后等待调用方去发现服务，实现代理转发到真正的业务系统，还可以基于服务发现做负载均衡，
    /// 甚至可以在客户端请求到底服务之前进行拦截，做一些基础性的工作，比如身份验证、限流、熔断等等业务系统的前瞻性工作。
    /// </remarks>
    public static class ConsulMildd
    {
        public static IApplicationBuilder UseConsulMildd(this IApplicationBuilder app, IConfiguration configuration, IHostApplicationLifetime lifetime)
        {
            if (configuration["Middleware:Consul:Enabled"].ToBoolReq())
            {
                var consulClient = new ConsulClient(c =>
               {
                   //consul地址
                   c.Address = new Uri(configuration["ConsulSetting:ConsulAddress"]);
               });

                var registration = new AgentServiceRegistration()
                {
                    ID = Guid.NewGuid().ToString(),//服务实例唯一标识
                    Name = configuration["ConsulSetting:ServiceName"],//服务名
                    Address = configuration["ConsulSetting:ServiceIP"], //服务IP
                    Port = int.Parse(configuration["ConsulSetting:ServicePort"]),//服务端口
                    Check = new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(7),// 服务启动 7 秒后注册服务
                        Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔
                        HTTP = $"http://{configuration["ConsulSetting:ServiceIP"]}:{configuration["ConsulSetting:ServicePort"]}{configuration["ConsulSetting:ServiceHealthCheck"]}",//健康检查地址
                        Timeout = TimeSpan.FromSeconds(5)//超时时间
                    }
                };

                //服务注册
                consulClient.Agent.ServiceRegister(registration).Wait();

                //应用程序终止时，取消注册
                lifetime.ApplicationStopping.Register(() =>
                {
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
            }
            return app;
        }
    }
}