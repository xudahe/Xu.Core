using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Services;

namespace Xu.Services
{
    public partial class DeptSvc : BaseSvc<Dept>, IDeptSvc
    {
        private readonly IDeptRepo _dal;

        public DeptSvc(IDeptRepo dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}