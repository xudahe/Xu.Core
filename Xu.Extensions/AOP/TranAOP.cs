using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xu.Common;
using Xu.Repository;

namespace Xu.Extensions
{
    /// <summary>
    /// 事务拦截器TranAOP 继承IInterceptor接口
    /// </summary>
    public class TranAOP : IInterceptor
    {
        private readonly ILogger<TranAOP> _logger;
        private readonly IUnitOfWorkManage _unitOfWorkManage;

        public TranAOP(IUnitOfWorkManage unitOfWorkManage, ILogger<TranAOP> logger)
        {
            _unitOfWorkManage = unitOfWorkManage;
            _logger = logger;
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
            if (method.GetCustomAttribute<UseTranAttribute>(true) is { } uta)
            {
                try
                {
                    Before(method, uta.Propagation);

                    invocation.Proceed();

                    // 异步获取异常，先执行
                    if (IsAsyncMethod(invocation.Method))
                    {
                        var result = invocation.ReturnValue;
                        if (result is Task)
                        {
                            Task.WaitAll(result as Task);
                        }
                    }

                    After(method);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    AfterException(method);
                    throw;
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
        }

        private void Before(MethodInfo method, Propagation propagation)
        {
            switch (propagation)
            {
                case Propagation.Required:
                    if (_unitOfWorkManage.TranCount <= 0)
                    {
                        _logger.LogDebug($"Begin Transaction");
                        Console.WriteLine($"Begin Transaction");
                        _unitOfWorkManage.BeginTran(method);
                    }

                    break;

                case Propagation.Mandatory:
                    if (_unitOfWorkManage.TranCount <= 0)
                    {
                        throw new Exception("事务传播机制为:[Mandatory],当前不存在事务");
                    }

                    break;

                case Propagation.Nested:
                    _logger.LogDebug($"Begin Transaction");
                    Console.WriteLine($"Begin Transaction");
                    _unitOfWorkManage.BeginTran(method);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propagation), propagation, null);
            }
        }

        private void After(MethodInfo method)
        {
            _unitOfWorkManage.CommitTran(method);
        }

        private void AfterException(MethodInfo method)
        {
            _unitOfWorkManage.RollbackTran(method);
        }

        private static async Task SuccessAction(IInvocation invocation)
        {
            await Task.Run(() =>
            {
                //...
            });
        }

        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }

        private static async Task TestActionAsync(IInvocation invocation)
        {
            await Task.Run(null);
        }
    }
}