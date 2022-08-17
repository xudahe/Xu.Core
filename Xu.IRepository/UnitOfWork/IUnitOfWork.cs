﻿using SqlSugar;

namespace Xu.IRepository
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// SqlSugarScope 实列
        /// </summary>
        SqlSugarScope GetDbClient();

        /// <summary>
        /// 开始事务
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTran();
    }
}