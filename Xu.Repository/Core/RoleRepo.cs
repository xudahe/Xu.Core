using Xu.IRepository;
using Xu.Model.Models;

namespace Xu.Repository
{
    public class RoleRepo : BaseRepo<Role>, IRoleRepo
    {
        public RoleRepo(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}