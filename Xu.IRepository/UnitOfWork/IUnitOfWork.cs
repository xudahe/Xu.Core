using SqlSugar;

namespace Xu.IRepository
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// 创建SqlSugarClient 实列
        /// </summary>
        SqlSugarClient GetDbClient();

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