using Microsoft.Extensions.Logging;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Repository;

namespace Xu.Services
{
    /// <summary>
	/// WeChatCompanyServices
	/// </summary>
    public class WeChatCompanyServices : BaseSvc<WeChatCompany>, IWeChatCompanyServices
    {
        private readonly IBaseRepo<WeChatCompany> _dal;
        private readonly IUnitOfWorkManage _unitOfWorkManage;
        private readonly ILogger<WeChatCompanyServices> _logger;

        public WeChatCompanyServices(IBaseRepo<WeChatCompany> dal, IUnitOfWorkManage unitOfWorkManage, ILogger<WeChatCompanyServices> logger)
        {
            this._dal = dal;
            base.BaseDal = dal;
            this._unitOfWorkManage = unitOfWorkManage;
            this._logger = logger;
        }
    }
}