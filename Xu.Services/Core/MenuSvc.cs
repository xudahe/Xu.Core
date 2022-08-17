using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services.Core
{
    public class MenuSvc : BaseSvc<Menu>, IMenuSvc
    {
        public MenuSvc(IBaseRepo<Menu> dalRepo)
        {
            base.BaseDal = dalRepo;
        }

        public async Task<List<Menu>> GetDataByids(string ids, List<Menu> list = null)
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

        public async Task<List<Menu>> GetDataBySystemId(string id, List<Menu> list = null)
        {
            list = list ?? await base.Query();

            if (GUIDHelper.IsGuidByReg(id))
                return list.Where(s => s.Guid == id).ToList();
            else
                return list.Where(s => s.Id == id.ToInt32Req()).ToList();
        }
    }
}