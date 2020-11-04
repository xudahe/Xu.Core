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
    /// 角色管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class RoleController : ControllerBase
    {
        private readonly IRoleSvc _roleSvc;

        public RoleController(IRoleSvc roleSvc)
        {
            _roleSvc = roleSvc;
        }

        /// <summary>
        /// 获取角色数据
        /// </summary>
        /// <param name="ids">可空</param>
        /// <param name="roleName">角色名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetRole(string ids, string roleName = "")
        {
            var data = await _roleSvc.Query();

            if (!string.IsNullOrEmpty(ids))
                data = data.Where(a => ids.SplitInt(",").Contains(a.Id)).ToList();

            if (!string.IsNullOrEmpty(roleName))
                data = data.Where(a => a.RoleName != null && a.RoleName.Contains(roleName)).ToList();

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
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> PostRole([FromBody] Role role)
        {
            var model = await _roleSvc.SaveRole(role);

            return new MessageModel<Role>()
            {
                Message = "添加成功",
                Success = true,
                Response = model
            };
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PutRole([FromBody] Role role)
        {
            var data = new MessageModel<string>();
            if (role != null && role.Id > 0)
            {
                data.Success = await _roleSvc.Update(role);
                if (data.Success)
                {
                    data.Message = "更新成功";
                    data.Response = role?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">非空</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeleteRole(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var role = await _roleSvc.QueryById(id);
                role.DeleteTime = DateTime.Now;
                data.Success = await _roleSvc.Update(role);
                if (data.Success)
                {
                    data.Message = "删除成功";
                    data.Response = role?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用角色
        /// </summary>
        /// <param name="id">非空</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DisableRole(int id, bool falg)
        {
            var role = await _roleSvc.QueryById(id);
            role.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _roleSvc.Update(role)
            };

            if (data.Success)
            {
                data.Message = falg ? "禁用成功" : "启用成功";
                data.Response = role?.Id.ToString();
            }

            return data;
        }
    }
}