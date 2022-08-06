using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    public partial class PlatformSvc : BaseSvc<Platform>, IPlatformSvc
    {
        public PlatformSvc(IBaseRepo<Platform> dalRepo)
        {
            base.BaseDal = dalRepo;
        }

        public async Task<List<Platform>> GetDataByids(string ids, List<Platform> list = null)
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