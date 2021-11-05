using RabbitMQ.Client;
using System;

namespace Xu.EventBus
{
    /// <summary>
    /// RabbitMQ持久连接
    /// 接口
    /// </summary>
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        /// <summary>
        /// 是否已连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        /// 创建Model
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}