using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Services;

namespace Xu.Services
{
    public partial class DeptSvc : BaseSvc<Dept>, IDeptSvc
    {
        IBaseRepo<Dept> _dal;

        public DeptSvc(IBaseRepo<Dept> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}