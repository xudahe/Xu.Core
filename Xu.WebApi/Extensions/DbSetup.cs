using Microsoft.Extensions.DependencyInjection;
using System;
using Xu.Model;

namespace Xu.WebApi
{
    /// <summary>
    /// Db 启动服务（自动化初始数据库）
    /// </summary>
    public static class DbSetup
    {
        public static void AddDbSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<DBSeed>();
            services.AddScoped<MyContext>();
        }
    }
}