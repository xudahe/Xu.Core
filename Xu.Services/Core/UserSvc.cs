using System.Collections.Generic;
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
        public UserSvc(IBaseRepo<User> dalRepo)
        {
            base.BaseDal = dalRepo;
        }

        // 定义一个锁，防止多线程
        private static readonly object locker = new object();

        [Caching(AbsoluteExpiration = 30)]
        public async Task<string> GetUserNameById(int id)
        {
            return ((await base.QueryById(id))?.LoginName);
        }

        public async Task<List<User>> GetDataByids(string ids, List<User> list = null)
        {
            // 当第一个线程执行的时候，会对locker对象 "加锁"，
            // 当其他线程执行的时候，会等待 locker 执行完解锁
            lock (locker)
            {
            }

            list = list ?? await base.Query();

            if (!string.IsNullOrEmpty(ids))
            {
                var idList = ids.Split(',');
                if (GUIDHelper.IsGuidByReg(idList[0]))
                    return list.Where(s => idList.Contains(s.Guid)).ToList();
                else
                    return list.Where(s => idList.ToInt32List().Contains(s.Id)).ToList();
            }

            return list;
        }
    }
}