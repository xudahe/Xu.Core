using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    public class TaskQzSvc : BaseSvc<TasksQz>, ITasksQzSvc
    {
        /// <summary>
        /// 仓储接口注入
        /// </summary>
        /// <param name="taskQzRepo"></param>
        public TaskQzSvc(IBaseRepo<TasksQz> dalRepo)
        {
            base.BaseDal = dalRepo;
        }
    }
}