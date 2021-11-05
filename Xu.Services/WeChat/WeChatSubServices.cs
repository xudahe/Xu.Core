using Microsoft.Extensions.Logging;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Services;

namespace Blog.Core.Services
{
    /// <summary>
	/// WeChatSubServices
	/// </summary>
    public class WeChatSubServices : BaseSvc<WeChatSub>, IWeChatSubServices
    {
        private readonly IBaseRepo<WeChatSub> _dal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WeChatSubServices> _logger;

        public WeChatSubServices(IBaseRepo<WeChatSub> dal, IUnitOfWork unitOfWork, ILogger<WeChatSubServices> logger)
        {
            this._dal = dal;
            base.BaseDal = dal;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }
    }
}