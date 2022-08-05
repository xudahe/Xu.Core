using Xu.IRepository;
using Xu.Model.IDS4DbModels;

namespace Xu.Services.IDS4Db
{
    public class ApplicationUserSvc : BaseSvc<ApplicationUser>, IApplicationUserSvc
    {
        private IBaseRepo<ApplicationUser> _dal;

        public ApplicationUserSvc(IBaseRepo<ApplicationUser> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}