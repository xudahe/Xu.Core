using Xu.Common.HttpPolly;
using Xu.Model;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Net.Http;
using Xu.EnumHelper;

namespace Xu.Extensions
{
    /// <summary>
    /// 集成Polly，处理HTTP请求过程中的瞬时故障
    /// </summary>
    public static class HttpPollySetup
    {
        public static void AddHttpPollySetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            #region Polly策略

            var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>() // 若超时则抛出此异常
            //该策略将处理典型的瞬态故障，如果需要，会最多重试 3 次 Http 请求。这个策略将在第一次重试前延迟 1 秒，第二次重试前延迟 5 秒，在第三次重试前延迟 10 秒。
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            });

            // 为每个重试定义超时策略，10秒超时
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);
            //有时我们也需要一个没有任何行为的策略，Polly系统默认提供了一个.
            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

            #endregion Polly策略

            services.AddHttpClient(HttpEnum.Common.ToString(), c =>
            {
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(retryPolicy)
            // 将超时策略放在重试策略之内，每次重试会应用此超时策略
            .AddPolicyHandler(timeoutPolicy);

            services.AddHttpClient(HttpEnum.LocalHost.ToString(), c =>
            {
                c.BaseAddress = new Uri("http://www.localhost.com");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(retryPolicy)
            // 将超时策略放在重试策略之内，每次重试会应用此超时策略
            .AddPolicyHandler(timeoutPolicy);

            services.AddSingleton<IHttpPollyHelper, HttpPollyHelper>();
        }
    }
}