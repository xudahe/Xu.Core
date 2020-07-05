using System.Threading.Tasks;
using Xu.Model;
using Xu.Model.Models;

namespace Xu.IServices
{
    public interface IUserSvc : IBaseSvc<User>
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<User> SaveUser(User user);

        /// <summary>
        /// 根据用户Id获取用户名
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        Task<string> GetUserNameById(int id);
    }
}