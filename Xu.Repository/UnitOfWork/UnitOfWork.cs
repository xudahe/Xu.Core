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

        //防止同一范围内，异步执行事务误触非当前线程事务
        private int _tranCount { get; set; }

        public UnitOfWork(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
            _tranCount = 0;
        }

        /// <summary>
        /// 获取DB，保证唯一性
        /// </summary>
        /// <returns></returns>
        public SqlSugarScope GetDbClient()
        {
            // 必须要as，后边会用到切换数据库操作
            return _sqlSugarClient as SqlSugarScope;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTran()
        {
            lock (this)
            {
                _tranCount++;
                GetDbClient().BeginTran();
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            lock (this)
            {
                _tranCount--;
                if (_tranCount == 0)
                {
                    try
                    {
                        GetDbClient().CommitTran();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        GetDbClient().RollbackTran();
                    }
                }
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            lock (this)
            {
                _tranCount--;
                GetDbClient().RollbackTran();
            }
        }
    }
}