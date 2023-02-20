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
    public class MenuController : ControllerBase
    {
        private readonly IMenuSvc _menuSvc;
        private readonly ISystemSvc _systemSvc;

        public MenuController(IMenuSvc menuSvc, ISystemSvc systemSvc)
        {
            _menuSvc = menuSvc;
            _systemSvc = systemSvc;
        }

        /// <summary>
        /// 获取菜单数据（列表）
        /// </summary>
        /// <param name="ids">菜单id或guid集合（可空）</param>
        /// <param name="menuName">菜单名称（可空）</param>
        /// <param name="parentId">父级菜单Id（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetMenu(string ids = "", string menuName = "", string parentId = "")
        {
            var data = await _menuSvc.GetDataByids(ids);

            for (int i = 0; i < data.Count; i++)
            {
                if (string.IsNullOrEmpty(data[i].ParentId)) continue;

                if (GUIDHelper.IsGuidByReg(data[i].ParentId))
                    data[i].ParentName = data.First(s => s.Guid == data[i].ParentId).MenuName;
                else
                    data[i].ParentName = data.First(s => s.Id == data[i].ParentId.ToInt32()).MenuName;
            }

            if (!string.IsNullOrEmpty(menuName))
                data = data.Where(a => a.MenuName.Contains(menuName)).ToList();

            if (!string.IsNullOrEmpty(parentId))
                data = data.Where(a => a.ParentId == parentId).ToList();

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
        public async Task<object> GetMenuByPage(int page = 1, int pageSize = 50, string menuName = "")
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
        /// <param name="ids">菜单id或guid集合（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetMenuByIds(string ids)
        {
            var menuData = await _menuSvc.Query();
            var menuList = await _menuSvc.GetDataByids(ids, menuData);

            var systemList = await _systemSvc.Query();

            for (int i = 0; i < menuList.Count; i++)
            {
                if (!string.IsNullOrEmpty(menuList[i].SystemId))
                {
                    if (GUIDHelper.IsGuidByReg(menuList[i].SystemId))
                        menuList[i].SystemName = systemList.First(s => s.Guid == menuList[i].SystemId).SystemName;
                    else
                        menuList[i].SystemName = systemList.First(s => s.Id == menuList[i].SystemId.ToInt32()).SystemName;
                }

                if (!string.IsNullOrEmpty(menuList[i].ParentId))
                {
                    if (GUIDHelper.IsGuidByReg(menuList[i].ParentId))
                        menuList[i].ParentName = menuData.First(s => s.Guid == menuList[i].ParentId).MenuName;
                    else
                        menuList[i].ParentName = menuData.First(s => s.Id == menuList[i].ParentId.ToInt32()).MenuName;
                }
            }

            //获取一级菜单
            var menuList1 = menuList.Where(s => string.IsNullOrEmpty(s.ParentId)).ToList();

            //获取二级菜单
            for (int i = 0; i < menuList1.Count; i++)
            {
                var menuList2 = menuList.Where(s => s.ParentId == menuList1[i].Guid).ToList();
                //var menuList2 = menuList.Where(s => s.ParentId == menuList1[i].Id.ToString()).ToList();

                //获取三级菜单
                for (int j = 0; j < menuList2.Count; j++)
                {
                    menuList2[j].Children = menuList.Where(s => s.ParentId == menuList2[j].Guid).ToList();
                    //menuList2[j].Children = menuList.Where(s => s.ParentId == menuList2[j].Id.ToString()).ToList();
                }

                menuList1[i].Children = menuList2;
            }

            return new MessageModel<object>()
            {
                Response = menuList1.OrderBy(s => s.Index).ToList(),
                Success = true,
                Message = "获取成功"
            };
        }

        /// <summary>
        /// 根据系统Id或guid 获取菜单数据（树状）
        /// </summary>
        /// <param name="systemId">系统Id或guid（非空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetMenuBySystemId(string systemId)
        {
            if (string.IsNullOrEmpty(systemId))
            {
                return new MessageModel<object>()
                {
                    Success = true,
                    Message = "系统Id或guid不能为空"
                };
            }

            var menuData = await _menuSvc.Query();
            var menuList = menuData.Where(s => s.SystemId == systemId).ToList();

            var systemList = await _systemSvc.Query();

            for (int i = 0; i < menuList.Count; i++)
            {
                if (!string.IsNullOrEmpty(menuList[i].SystemId))
                {
                    if (GUIDHelper.IsGuidByReg(menuList[i].SystemId))
                        menuList[i].SystemName = systemList.First(s => s.Guid == menuList[i].SystemId).SystemName;
                    else
                        menuList[i].SystemName = systemList.First(s => s.Id == menuList[i].SystemId.ToInt32()).SystemName;
                }

                if (!string.IsNullOrEmpty(menuList[i].ParentId))
                {
                    if (GUIDHelper.IsGuidByReg(menuList[i].ParentId))
                        menuList[i].ParentName = menuData.First(s => s.Guid == menuList[i].ParentId).MenuName;
                    else
                        menuList[i].ParentName = menuData.First(s => s.Id == menuList[i].ParentId.ToInt32()).MenuName;
                }
            }

            //获取一级菜单
            var menuList1 = menuList.Where(s => string.IsNullOrEmpty(s.ParentId)).ToList();

            //获取二级菜单
            for (int i = 0; i < menuList1.Count; i++)
            {
                var menuList2 = menuList.Where(s => s.ParentId == menuList1[i].Guid).ToList();
                //var menuList2 = menuList.Where(s => s.ParentId == menuList1[i].Id.ToString()).ToList();

                //获取三级菜单
                for (int j = 0; j < menuList2.Count; j++)
                {
                    menuList2[j].Children = menuList.Where(s => s.ParentId == menuList2[j].Guid).ToList();
                    //menuList2[j].Children = menuList.Where(s => s.ParentId == menuList2[j].Id.ToString()).ToList();
                }

                menuList1[i].Children = menuList2;
            }

            return new MessageModel<object>()
            {
                Response = menuList1.OrderBy(s => s.Index).ToList(),
                Success = true,
                Message = "获取成功"
            };
        }

        /// <summary>
        /// 获取一级菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetParentMenu()
        {
            //获取一级菜单
            var menuList1 = (await _menuSvc.Query()).Where(s => string.IsNullOrEmpty(s.ParentId)).ToList();

            return new MessageModel<object>()
            {
                Response = menuList1.OrderBy(s => s.Index).ToList(),
                Success = true,
                Message = "获取成功"
            };
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> PostMenu([FromBody] Menu model)
        {
            var data = new MessageModel<Menu>() { Message = "添加成功", Success = true };

            var dataList = await _menuSvc.Query(a => a.MenuName == model.MenuName);
            if (dataList.Count > 0)
            {
                data.Message = "该菜单已存在";
                data.Success = false;
            }
            else
            {
                model.Id = await _menuSvc.Add(model);
                data.Response = model;
                data.Message = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PutMenu([FromBody] Menu model)
        {
            var data = new MessageModel<string>();
            if (model != null && model.Id > 0)
            {
                data.Success = await _menuSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "更新成功";
                }
            }

            return data;
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id">菜单id（非空）</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeleteMenu(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var model = await _menuSvc.QueryById(id);
                model.DeleteTime = DateTime.Now;
                data.Success = await _menuSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "删除成功";
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用菜单
        /// </summary>
        /// <param name="id">菜单id（非空）</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DisableMenu(int id, bool falg)
        {
            var model = await _menuSvc.QueryById(id);
            model.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _menuSvc.Update(model)
            };

            if (data.Success)
            {
                data.Message = falg ? "禁用成功" : "启用成功";
            }

            return data;
        }
    }
}