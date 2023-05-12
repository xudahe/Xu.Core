using InitQ.Abstractions;
using InitQ.Attributes;
using System;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.Extensions
{
    public class RedisSubscribe : IRedisSubscribe
    {
        public RedisSubscribe()
        {
        }

        [Subscribe(RedisMqKey.Loging)]
        private async Task SubRedisLoging(string msg)
        {
            Console.WriteLine($"订阅者 1 从 队列{RedisMqKey.Loging} 消费到/接受到 消息:{msg}");

            await Task.CompletedTask;
        }
    }
}