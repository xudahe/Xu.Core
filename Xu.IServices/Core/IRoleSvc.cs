using System.Threading.Tasks;
using Xu.Model;
using Xu.Model.Models;

namespace Xu.IServices
{
    public interface IRoleSvc : IBaseSvc<Role>
    {
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<Role> SaveRole(Role role);

        /// <summary>
        /// 根据角色Id获取角色名称
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        Task<string> GetRoleNameById(int id);
    }
}