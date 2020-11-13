using SqlSugar;
using System;
using Xu.IRepository;

/// <summary>
/// 工作单元，对事务行为做实现
/// </summary>
namespace Xu.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public UnitOfWork(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        /// <summary>
        /// 获取DB，保证唯一性
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient GetDbClient()
        {
            // 必须要as，后边会用到切换数据库操作
            return _sqlSugarClient as SqlSugarClient;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTran()
        {
            GetDbClient().BeginTran();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            try
            {
                GetDbClient().CommitTran();
            }
            catch (Exception ex)
            {
                GetDbClient().RollbackTran();
                throw ex;
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            GetDbClient().RollbackTran();
        }
    }
}