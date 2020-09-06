using Xu.Model.Enum;
using Xu.Model.Models;

namespace Xu.IServices
{
    /// <summary>
    /// 待办ISvc
    /// </summary>
    public interface ITodoSvc : IBaseSvc<Todo>
    {
        /// <summary>
        /// 添加待办
        /// </summary>
        /// <param name="relatedDomain">关联领域对象</param>
        /// <param name="relatedDomainId">关联领域对象Id</param>
        /// <param name="sendActorType">发送人类型</param>
        /// <param name="sendActorId">发送人Id</param>
        /// <param name="receiveActorType">接收人类型</param>
        /// <param name="receiveActorId">接收人Id</param>
        /// <param name="title">待办标题</param>
        /// <param name="level">紧急程度</param>
        /// <param name="type">待办类型</param>
        /// <param name="additionalData">附加数据</param>
        void Send(string relatedDomain, string relatedDomainId
            , string sendActorType, string sendActorId
            , string receiveActorType, string receiveActorId
            , string title, TodoLevel level = TodoLevel.一般, TodoType type = TodoType.待办
            , string additionalData = "");

        /// <summary>
        /// 处理待办
        /// </summary>
        /// <param name="id"></param>
        void Done(int id);
    }
}