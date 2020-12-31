using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Model.Models;

namespace Xu.IServices
{
    public interface IUserSvc : IBaseSvc<User>
    {
        /// <summary>
        /// 根据用户Id获取用户名
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        Task<string> GetUserNameById(int id);

        /// <summary>
        /// 根据用户id或guid集合 过滤数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<User>> GetDataByids(string ids, List<User> list = null);
    }
}