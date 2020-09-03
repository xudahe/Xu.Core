using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    /// <summary>
    /// Role
    /// </summary>
    public class RoleSvc : BaseSvc<Role>, IRoleSvc
    {
        public RoleSvc(IBaseRepo<Role> dalRepo)
        {
            base.BaseDal = dalRepo;
        }

        public async Task<Role> SaveRole(Role role)
        {
            Role model = new Role();
            var userList = await base.Query(a => a.RoleName == role.RoleName);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(role);
                model = await base.QueryById(id);
            }

            return model;
        }

        [Caching(AbsoluteExpiration = 30)]
        public async Task<string> GetRoleNameById(int id)
        {
            return ((await base.QueryById(id))?.RoleName);
        }
    }
}