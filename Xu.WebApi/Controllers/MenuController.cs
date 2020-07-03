using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class MenuController
    {
        private readonly IMenuSvc _menuSvc;

        public MenuController(IMenuSvc menuSvc)
        {
            _menuSvc = menuSvc;
        }

        /// <summary>
        /// 获取全部菜单并分页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="menuName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get(int page = 1, int pageSize = 50, string menuName = "")
        {
            var data = await _menuSvc.QueryPage(a => a.DeleteTime == null && ((a.MenuName != null && a.MenuName.Contains(menuName))), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<Menu>>()
            {
                Msg = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 根据菜单Ids集合获取菜单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<object> GetByIds(string ids)
        {
            var menuList = new List<Menu>();
            if (string.IsNullOrEmpty(ids)) {
                menuList = await _menuSvc.Query(s => s.Enabled == false);
            }
            else
            {
                var menuIds = ids.SplitInt(",");
                menuList = await _menuSvc.Query(s => s.Enabled == false && menuIds.Contains(s.Id));
            }

            var menuList1 = menuList.Where(s => !s.ParentId.HasValue).OrderBy(s=>s.Index).ToList(); //获取一级菜单（顶部）

            IDictionary<Menu, object> dic1 = new Dictionary<Menu, object>();
            for (int i = 0; i < menuList1.Count(); i++)
            {
                var menuList2 = menuList.Where(s => s.ParentId == menuList1[i].Id).OrderBy(s => s.Index).ToList(); //获取二级菜单

                IDictionary<Menu, object> dic2 = new Dictionary<Menu, object>();
                for (int j = 0; j < menuList2.Count(); j++)
                {
                    var menuList3 = menuList.Where(s => s.ParentId == menuList2[j].Id).OrderBy(s => s.Index).ToList(); //获取三级菜单
                    dic2.Add(menuList2[i], menuList3);
                }
                dic1.Add(menuList1[i], dic2);
            }
                
            return new MessageModel<object>() {
                Response = dic1,
                Success = dic1.Count >= 0,
                Msg = "获取成功"
            };
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> Post([FromBody] Menu menu)
        {
            var model = await _menuSvc.SaveMenu(menu);

            return new MessageModel<Menu>()
            {
                Msg = "添加成功",
                Success = true,
                Response = model
            };
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> Put([FromBody] Menu menu)
        {
            var data = new MessageModel<string>();
            if (menu != null && menu.Id > 0)
            {
                data.Success = await _menuSvc.Update(menu);
                if (data.Success)
                {
                    data.Msg = "更新成功";
                    data.Response = menu?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var menu = await _menuSvc.QueryById(id);
                menu.DeleteTime = DateTime.Now;
                data.Success = await _menuSvc.Update(menu);
                if (data.Success)
                {
                    data.Msg = "删除成功";
                    data.Response = menu?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用菜单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="falg"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> Disable(int id, bool falg)
        {
            var menu = await _menuSvc.QueryById(id);
            menu.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _menuSvc.Update(menu)
            };

            if (data.Success)
            {
                data.Msg = falg ? "禁用成功" : "启用成功";
                data.Response = menu?.Id.ToString();
            }

            return data;
        }
    }
}