using System.Linq;
using System.Threading.Tasks;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services.Core
{
    public class MenuSvc : BaseSvc<Menu>, IMenuSvc
    {
        public MenuSvc(IMenuRepo menuRepo)
        {
            base.BaseDal = menuRepo;
        }

        public async Task<Menu> SaveMenu(Menu menu)
        {
            Menu model = new Menu();
            var menuList = await base.Query(a => a.ClassName == menu.ClassName);
            if (menuList.Count > 0)
            {
                model = menuList.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(menu);
                model = await base.QueryById(id);
            }

            return model;
        }
    }
}