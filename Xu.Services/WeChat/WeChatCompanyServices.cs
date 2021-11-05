using Microsoft.Extensions.Logging;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    /// <summary>
	/// WeChatCompanyServices
	/// </summary>
    public class WeChatCompanyServices : BaseSvc<WeChatCompany>, IWeChatCompanyServices
    {
        private readonly IBaseRepo<WeChatCompany> _dal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WeChatCompanyServices> _logger;

        public WeChatCompanyServices(IBaseRepo<WeChatCompany> dal, IUnitOfWork unitOfWork, ILogger<WeChatCompanyServices> logger)
        {
            this._dal = dal;
            base.BaseDal = dal;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }
    }
}