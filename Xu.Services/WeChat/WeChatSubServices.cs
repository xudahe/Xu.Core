using Microsoft.Extensions.Logging;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Repository;
using Xu.Services;

namespace Blog.Core.Services
{
    /// <summary>
	/// WeChatSubServices
	/// </summary>
    public class WeChatSubServices : BaseSvc<WeChatSub>, IWeChatSubServices
    {
        private readonly IBaseRepo<WeChatSub> _dal;
        private readonly IUnitOfWorkManage _unitOfWorkManage;
        private readonly ILogger<WeChatSubServices> _logger;

        public WeChatSubServices(IBaseRepo<WeChatSub> dal, IUnitOfWorkManage unitOfWorkManage, ILogger<WeChatSubServices> logger)
        {
            this._dal = dal;
            base.BaseDal = dal;
            this._unitOfWorkManage = unitOfWorkManage;
            this._logger = logger;
        }
    }
}