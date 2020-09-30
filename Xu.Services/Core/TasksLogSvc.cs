using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    public class TasksLogSvc : BaseSvc<TasksLog>, ITasksLogSvc
    {
        /// <summary>
        /// 仓储接口注入
        /// </summary>
        /// <param name="taskQzRepo"></param>
        public TasksLogSvc(IBaseRepo<TasksLog> dalRepo)
        {
            base.BaseDal = dalRepo;
        }
    }
}