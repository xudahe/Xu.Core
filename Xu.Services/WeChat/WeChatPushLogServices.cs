using Microsoft.Extensions.Logging;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Services;

namespace Blog.Core.Services
{
    /// <summary>
	/// WeChatPushLogServices
	/// </summary>
    public class WeChatPushLogServices : BaseSvc<WeChatPushLog>, IWeChatPushLogServices
    {
        private readonly IBaseRepo<WeChatPushLog> _dal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WeChatPushLogServices> _logger;

        public WeChatPushLogServices(IBaseRepo<WeChatPushLog> dal, IUnitOfWork unitOfWork, ILogger<WeChatPushLogServices> logger)
        {
            this._dal = dal;
            base.BaseDal = dal;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }
    }
}