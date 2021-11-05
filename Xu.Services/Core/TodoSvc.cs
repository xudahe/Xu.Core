using System;
using Xu.EnumHelper;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    /// <summary>
    /// 待办Svc
    /// </summary>
    public class TodoSvc : BaseSvc<Todo>, ITodoSvc
    {
        /// <summary>
        /// 仓储接口注入
        /// </summary>
        /// <param name="taskQzRepo"></param>
        public TodoSvc(IBaseRepo<Todo> dalRepo)
        {
            base.BaseDal = dalRepo;
        }

        private static readonly object _lock = new object();

        public void Send(string relatedDomain, string relatedDomainId
            , string sendActorType, string sendActorId
            , string receiveActorType, string receiveActorId
            , string title, TodoLevel level = TodoLevel.一般, TodoType type = TodoType.待办
            , string additionalData = "")
        {
            lock (_lock)
            {
                Todo todo = new Todo
                {
                    RelatedDomain = relatedDomain,
                    RelatedDomainId = relatedDomainId,
                    SendActorType = sendActorType,
                    SendActorId = sendActorId,
                    Title = title.Replace("有关于《", "有《"),
                    Level = level,
                    Type = type,
                    ActionType = "",
                    AdditionalData = additionalData,
                    ReceiveTime = DateTime.Now,
                    ReceiveActorType = receiveActorType,
                    ReceiveActorId = receiveActorId,
                    StandardDate = null
                };

                base.BaseDal.Add(todo);
            }
        }

        public void Done(int id)
        {
            lock (_lock)
            {
                DateTime actionTime = DateTime.Now;

                var todo = base.BaseDal.QueryById(id).Result;
                if (todo != null && !todo.IsAction)
                {
                    todo.ActionTime = actionTime;
                    base.BaseDal.Update(todo);
                }
            }
        }
    }
}