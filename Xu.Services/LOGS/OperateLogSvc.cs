using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    public partial class AccessTrendLogSvc : BaseSvc<AccessTrendLog>, IAccessTrendLogSvc
    {
        private IBaseRepo<AccessTrendLog> _dal;

        public AccessTrendLogSvc(IBaseRepo<AccessTrendLog> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}