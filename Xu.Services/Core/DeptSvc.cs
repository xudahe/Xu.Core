using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    public partial class DeptSvc : BaseSvc<Dept>, IDeptSvc
    {
        public DeptSvc(IBaseRepo<Dept> dalRepo)
        {
            base.BaseDal = dalRepo;
        }
    }
}