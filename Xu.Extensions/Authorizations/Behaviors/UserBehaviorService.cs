using System.Threading.Tasks;

namespace Xu.Extensions.Authorizations.Behaviors
{
    public class UserBehaviorService : IUserBehaviorService
    {
        public UserBehaviorService()
        {
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