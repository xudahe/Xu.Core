using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Model.ResultModel;

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
        /// 获取菜单数据（列表）
        /// </summary>
        /// <param name="ids">可空</param>
        /// <param name="menuName">菜单名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get(string ids, string menuName = "")
        {
            var data = await _menuSvc.Query();

            if (!string.IsNullOrEmpty(ids))
                data = data.Where(a => ids.SplitInt(",").Contains(a.Id)).ToList();

            if (!string.IsNullOrEmpty(menuName))
                data = data.Where(a => a.MenuName.Contains(menuName)).ToList();

            return new MessageModel<List<Menu>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 获取全部菜单并分页
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="menuName">菜单名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetByPage(int page = 1, int pageSize = 50, string menuName = "")
        {
            var data = await _menuSvc.QueryPage(a => (a.MenuName != null && a.MenuName.Contains(menuName)), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<Menu>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 根据菜单Ids集合获取菜单数据（树状）
        /// </summary>
        /// <param name="ids">可空</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetByIds(string ids)
        {
            var menuList = await _menuSvc.Query(s => s.Enabled == false);

            var menuList1 = new List<Menu>();
            if (string.IsNullOrEmpty(ids))
            {
                menuList1 = menuList.Where(s => !s.ParentId.HasValue).OrderBy(s => s.Index).ToList(); //获取一级菜单（顶部）

                for (int i = 0; i < menuList1.Count(); i++)
                {
                    var menuList2 = menuList.Where(s => s.ParentId == menuList1[i].Id).OrderBy(s => s.Index).ToList(); //获取二级菜单

                    for (int j = 0; j < menuList2.Count(); j++)
                    {
                        menuList2[j].Children = menuList.Where(s => s.ParentId == menuList2[j].Id).OrderBy(s => s.Index).ToList(); //获取三级菜单
                    }
                    menuList1[i].Children = menuList2;
                }
            }
            else
            {
                var menuList3 = menuList.Where(s => ids.SplitInt(",").Contains(s.Id)).ToList();
                var menuList2 = menuList.Where(s => menuList3.Select(d => d.ParentId).Contains(s.Id)).ToList();
                menuList1 = menuList.Where(s => menuList2.Select(d => d.ParentId).Contains(s.Id)).ToList();

                for (int i = 0; i < menuList1.Count(); i++)
                {
                    var menuList_2 = menuList2.Where(s => s.ParentId == menuList1[i].Id).OrderBy(s => s.Index).ToList(); //获取二级菜单

                    for (int j = 0; j < menuList_2.Count(); j++)
                    {
                        menuList_2[j].Children = menuList3.Where(s => s.ParentId == menuList_2[j].Id).OrderBy(s => s.Index).ToList(); //获取三级菜单
                    }
                    menuList1[i].Children = menuList_2;
                }
            }

            return new MessageModel<object>()
            {
                Response = menuList1,
                Success = true,
                Message = "获取成功"
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
                Message = "添加成功",
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
                    data.Message = "更新成功";
                    data.Response = menu?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id">非空</param>
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
                    data.Message = "删除成功";
                    data.Response = menu?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用菜单
        /// </summary>
        /// <param name="id">非空</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
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
                data.Message = falg ? "禁用成功" : "启用成功";
                data.Response = menu?.Id.ToString();
            }

            return data;
        }
    }
}