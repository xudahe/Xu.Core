using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    public partial class OperateLogServices : BaseSvc<OperateLog>, IOperateLogSvc
    {
        private IBaseRepo<OperateLog> _dal;

        public OperateLogServices(IBaseRepo<OperateLog> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}