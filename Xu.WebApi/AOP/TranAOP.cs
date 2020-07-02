using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IRepository;

namespace Xu.WebApi
{
    /// <summary>
    /// 事务拦截器TranAOP 继承IInterceptor接口
    /// 建立事务AOP，解决多仓储内的事务操作
    /// </summary>
    public class TranAOP : IInterceptor
    {
        private readonly IUnitOfWork _unitOfWork;

        public TranAOP(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 实例化IInterceptor唯一方法
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //对当前方法的特性验证
            //如果需要验证
            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(UseTranAttribute)) is UseTranAttribute)
            {
                try
                {
                    Console.WriteLine($"Begin Transaction");

                    _unitOfWork.BeginTran();

                    invocation.Proceed();

                    //异步获取异常，普通的 try catch 外层不能达到目的，毕竟是异步的
                    if (IsAsyncMethod(invocation.Method))
                    {
                        if (invocation.Method.ReturnType == typeof(Task))
                        {
                            invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                                (Task)invocation.ReturnValue,
                                async () => await TestActionAsync(invocation),
                                ex =>
                                {
                                    _unitOfWork.RollbackTran();
                                });
                        }
                        else //Task<TResult>
                        {
                            invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                             invocation.Method.ReturnType.GenericTypeArguments[0],
                             invocation.ReturnValue,
                             async () => await TestActionAsync(invocation),
                             ex =>
                             {
                                 _unitOfWork.RollbackTran();
                             });
                        }
                    }
                    _unitOfWork.CommitTran();
                }
                catch (Exception)
                {
                    Console.WriteLine($"Rollback Transaction");
                    _unitOfWork.RollbackTran();
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
        }

        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }

        private async Task TestActionAsync(IInvocation invocation)
        {
        }
    }
}