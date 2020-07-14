using Xu.IRepository;
using Xu.Model.Models;
using Xu.Repository;

namespace Xu.Repository
{
    /// <summary>
    /// deptRepository
    /// </summary>
    public class deptRepo : BaseRepo<Dept>, IDeptRepo
    {
        public deptRepo(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}