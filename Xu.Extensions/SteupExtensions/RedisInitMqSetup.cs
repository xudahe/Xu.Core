﻿using InitQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// Redis 消息队列 启动服务
    /// </summary>
    public static class RedisInitMqSetup
    {
        public static void AddRedisInitMqSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (Appsettings.App(new string[] { "Startup", "RedisMq", "Enabled" }).ToBoolReq())
            {
                //
                services.AddInitQ(m =>
                {
                    //时间间隔
                    m.SuspendTime = 2000;
                    //redis服务器地址
                    m.ConnectionString = Appsettings.App(new string[] { "Redis", "ConnectionString" });
                    //对应的订阅者类，需要new一个实例对象，当然你也可以传参，比如日志对象
                    m.ListSubscribe = new List<Type>()
                    {
                        //typeof(RedisSubscribe),
                        //typeof(RedisSubscribe2)
                    };
                    //显示日志
                    m.ShowLog = false;
                });
            }
        }
    }
}