using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Model.Models;

namespace Xu.IServices
{
    public interface IRoleSvc : IBaseSvc<Role>
    {
        /// <summary>
        /// 根据角色Id获取角色名称
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        Task<string> GetRoleNameById(int id);

        /// <summary>
        /// 根据角色id或guid集合 过滤数据
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<List<Role>> GetDataByids(string ids, List<Role> list = null);
    }
}