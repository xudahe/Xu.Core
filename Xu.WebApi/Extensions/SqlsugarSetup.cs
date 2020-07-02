using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using Xu.Common;

namespace Xu.WebApi
{
    /// <summary>
    /// SqlSugar 启动服务
    /// </summary>
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<SqlSugar.ISqlSugarClient>(o =>
            {
                SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = BaseDBConfig.ConnectionString,//必填, 数据库连接字符串
                    DbType = (SqlSugar.DbType)BaseDBConfig.DbType,//必填, 数据库类型
                    IsAutoCloseConnection = true,//是否关闭数据库连接, 设置为true无需使用using或者Close操作
                    InitKeyType = SqlSugar.InitKeyType.SystemTable//默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
                });

                db.QueryFilter
                  .Add(new SqlFilterItem()//单表全局过滤器
                  {
                      FilterValue = filterDb =>
                      {
                          return new SqlFilterResult() { Sql = " DeleteTime IS NULL" };
                      },
                      IsJoinQuery = false
                  });

                return db;
            });
        }
    }
}