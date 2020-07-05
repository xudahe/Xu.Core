using System.Threading.Tasks;
using Xu.Model;
using Xu.Model.Models;

namespace Xu.IServices
{
    public interface IMenuSvc : IBaseSvc<Menu>
    {
        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        Task<Menu> SaveMenu(Menu menu);
    }
}