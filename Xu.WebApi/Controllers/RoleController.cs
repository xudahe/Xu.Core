using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model;
using Xu.Model.Models;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class RoleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRoleSvc _roleSvc;
        private readonly IMenuSvc _menuSvc;

        public RoleController(IMapper mapper, IRoleSvc roleSvc, IMenuSvc menuSvc)
        {
            _mapper = mapper;
            _roleSvc = roleSvc;
            _menuSvc = menuSvc;
        }

        /// <summary>
        /// 获取角色数据
        /// </summary>
        /// <param name="ids">角色id或guid集合（可空）</param>
        /// <param name="key">角色名称或角色简码（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetRole(string ids = "", string key = "")
        {
            var data = await _roleSvc.GetDataByids(ids);

            if (!string.IsNullOrEmpty(key))
                data = data.Where(a => a.RoleName.Contains(key) || a.RoleCode.Contains(key)).ToList();

            return new MessageModel<List<Role>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 获取全部角色并分页
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="roleName">角色名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetRoleByPage(int page = 1, int pageSize = 50, string roleName = "")
        {
            var data = await _roleSvc.QueryPage(a => (a.RoleName != null && a.RoleName.Contains(roleName)), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<Role>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> PostRole([FromBody] Role model)
        {
            var data = new MessageModel<Role>() { Message = "添加成功", Success = true };

            var dataList = await _roleSvc.Query(a => a.RoleName == model.RoleName);
            if (dataList.Count > 0)
            {
                data.Message = "该角色已存在";
                data.Success = false;
            }
            else
            {
                model.Id = await _roleSvc.Add(model);
                data.Response = model;
            }

            return data;
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PutRole([FromBody] Role model)
        {
            var data = new MessageModel<string>();

            var menuList = await _menuSvc.GetDataByids(model.MenuIds);
            model.MenuInfoList = _mapper.Map<IList<Menu>, IList<InfoMenu>>(menuList);

            if (model != null && model.Id > 0)
            {
                model.ModifyTime = DateTime.Now;
                data.Success = await _roleSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "更新成功";
                    data.Response = model.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色id（非空）</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeleteRole(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var model = await _roleSvc.QueryById(id);
                model.DeleteTime = DateTime.Now;
                data.Success = await _roleSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "删除成功";
                    data.Response = model.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用角色
        /// </summary>
        /// <param name="id">角色id（非空）</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DisableRole(int id, bool falg)
        {
            var model = await _roleSvc.QueryById(id);
            model.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _roleSvc.Update(model)
            };

            if (data.Success)
            {
                data.Message = falg ? "禁用成功" : "启用成功";
                data.Response = model.Id.ToString();
            }

            return data;
        }
    }
}