using Xu.IRepository;
using Xu.Model.Models;

namespace Xu.Repository
{
    public class MenuRepo : BaseRepo<Menu>, IMenuRepo
    {
        public MenuRepo(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}