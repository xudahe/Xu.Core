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
        /// <param name="isParent"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetByIds(string ids, string isParent = "")
        {
            var data = new MessageModel<List<Menu>>();
            if (!string.IsNullOrEmpty(ids))
            {
                var menuList = await _menuSvc.QueryByIds(ids.Split(","));

                if (menuList.Count > 0 && !string.IsNullOrEmpty(isParent))
                {
                    if (isParent.ToBoolReq())
                        menuList = menuList.Where(s => string.IsNullOrEmpty(s.ParentName)).ToList();
                    else
                        menuList = menuList.Where(s => !string.IsNullOrEmpty(s.ParentName)).ToList();
                }

                data.Response = menuList;
                data.Success = menuList.Count >= 0;
                data.Msg = "获取成功";
            }

            return data;
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