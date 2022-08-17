using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xu.IServices;

namespace Xu.Extensions.Authorizations.Behaviors
{
    public class UserBehaviorService : IUserBehaviorService
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly IUserSvc _userSvc;
        private readonly ILogger<UserBehaviorService> _logger;
        private readonly string _uid;
        private readonly string _token;

        public UserBehaviorService(IAspNetUser aspNetUser
            , IUserSvc userSvc
            , ILogger<UserBehaviorService> logger)
        {
            _aspNetUser = aspNetUser;
            _userSvc = userSvc;
            _logger = logger;
            _uid = aspNetUser.ID.ToString();
            _token = aspNetUser.GetToken();
        }

        public Task<bool> CheckTokenIsNormal()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CheckUserIsNormal()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CreateOrUpdateUserAccessByUid()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveAllUserAccessByUid()
        {
            throw new System.NotImplementedException();
        }
    }
}