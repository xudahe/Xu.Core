using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IRepository;

namespace Xu.Repository
{
    /// <summary>
    /// 工作单元，对事务行为做实现
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        // 注入 sugar client 实例
        public UnitOfWork(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;

            //记录项目启动请求api并访问service后，所有的db操作日志，包括Sql参数与Sql语句。
            if (Appsettings.App(new string[] { "AppSettings", "SqlAOP", "Enabled" }).ObjToBool())
            {
                sqlSugarClient.Aop.OnLogExecuting = (sql, pars) => //SQL执行中事件
                {
                    Parallel.For(0, 1, e =>
                    {
                        MiniProfiler.Current.CustomTiming("SQL：", GetParas(pars) + "【SQL语句】：" + sql);
                        LogLock.OutSql2Log("SqlLog", new string[] { GetParas(pars), "【SQL语句】：" + sql });
                    });
                };
            }
        }

        private string GetParas(SugarParameter[] pars)
        {
            string key = "【SQL参数】：";
            foreach (var param in pars)
            {
                key += $"{param.ParameterName}:{param.Value}\n";
            }

            return key;
        }

        /// <summary>
        /// 保证每次 scope 访问，多个仓储类，都用一个 client 实例
        //  注意，不是单例模型！！！
        /// </summary>
        /// <returns></returns>
        public ISqlSugarClient GetDbClient()
        {
            return _sqlSugarClient;
        }

        public void BeginTran()
        {
            GetDbClient().Ado.BeginTran();
        }

        public void CommitTran()
        {
            try
            {
                GetDbClient().Ado.CommitTran();
            }
            catch (Exception ex)
            {
                GetDbClient().Ado.RollbackTran();
                throw ex;
            }
        }

        public void RollbackTran()
        {
            GetDbClient().Ado.RollbackTran();
        }
    }
}