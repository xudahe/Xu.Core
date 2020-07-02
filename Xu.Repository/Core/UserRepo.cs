using Xu.IRepository;
using Xu.Model;

namespace Xu.Repository
{
    public class UserRepo : BaseRepo<User>, IUserRepo
    {
        public UserRepo(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}