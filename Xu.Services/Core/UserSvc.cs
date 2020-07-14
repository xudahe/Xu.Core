using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    /// <summary>
    ///
    /// </summary>
    public class UserSvc : BaseSvc<User>, IUserSvc
    {
        public UserSvc(IUserRepo userRepo)
        {
            base.BaseDal = userRepo;
        }

        // 定义一个锁，防止多线程
        private static readonly object locker = new object();

        public async Task<User> SaveUser(User user)
        {
            // 当第一个线程执行的时候，会对locker对象 "加锁"，
            // 当其他线程执行的时候，会等待 locker 执行完解锁
            lock (locker)
            {
            }

            User model = new User();
            var userList = await base.Query(a => a.LoginName == user.LoginName);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(user);
                model = await base.QueryById(id);
            }

            return model;
        }

        [Caching(AbsoluteExpiration = 30)]
        public async Task<string> GetUserNameById(int id)
        {
            return ((await base.QueryById(id))?.LoginName);
        }
    }
}