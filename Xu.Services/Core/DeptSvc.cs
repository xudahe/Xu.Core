using System.Linq;
using System.Threading.Tasks;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    public partial class DeptSvc : BaseSvc<Dept>, IDeptSvc
    {
        public DeptSvc(IBaseRepo<Dept> dalRepo)
        {
            base.BaseDal = dalRepo;
        }

        public async Task<Dept> SaveDept(Dept dept)
        {
            Dept model = new Dept();
            var dataList = await base.Query(a => a.DeptName == dept.DeptName);
            if (dataList.Count > 0)
            {
                model = dataList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(dept);
                model = await base.QueryById(id);
            }

            return model;
        }
    }
}