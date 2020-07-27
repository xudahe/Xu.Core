using Xu.IRepository;
using Xu.Model.Models;
using Xu.Repository;

namespace Xu.Repository
{
    public class DeptRepo : BaseRepo<Dept>, IDeptRepo
    {
        public DeptRepo(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}