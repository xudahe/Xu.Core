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
    /// Role
    /// </summary>
    public class RoleSvc : BaseSvc<Role>, IRoleSvc
    {
        public RoleSvc(IBaseRepo<Role> dalRepo)
        {
            base.BaseDal = dalRepo;
        }

        [Caching(AbsoluteExpiration = 30)]
        public async Task<string> GetRoleNameById(int id)
        {
            return ((await base.QueryById(id))?.RoleName);
        }

        public async Task<List<Role>> GetDataByids(string ids, List<Role> list = null)
        {
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