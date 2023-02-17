using System.Reflection;
using SqlSugar;

namespace Xu.Repository
{
    public interface IUnitOfWorkManage
    {
        /// <summary>
        /// SqlSugarScope 实列
        /// </summary>
        SqlSugarScope GetDbClient();

        int TranCount { get; }

        UnitOfWork CreateUnitOfWork();

        /// <summary>
        /// 开始事务
        /// </summary>
        void BeginTran();
        void BeginTran(MethodInfo method);

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTran();
        void CommitTran(MethodInfo method);

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTran();
        void RollbackTran(MethodInfo method);
    }
}