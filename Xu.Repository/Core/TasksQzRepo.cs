using Xu.IRepository;
using Xu.Model.Models;

namespace Xu.Repository
{
    public class TasksQzRepo : BaseRepo<TasksQz>, ITasksQzRepo
    {
        public TasksQzRepo(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}