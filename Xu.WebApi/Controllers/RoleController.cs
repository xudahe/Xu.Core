using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class RoleController
    {
        private readonly IRoleSvc _roleSvc;

        public RoleController(IRoleSvc roleSvc)
        {
            _roleSvc = roleSvc;
        }

        /// <summary>
        /// 获取全部角色并分页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<object> Get(int page = 1, int pageSize = 50, string key = "")
        {
            var data = await _roleSvc.QueryPage(a => (a.RoleName != null && a.RoleName.Contains(key) && (a.RoleCode != null && a.RoleCode.Contains(key))), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<Role>>()
            {
                Msg = "获取成功",
                Success = data.DataCount >= 0,
                Response = data
            };
        }

        /// <summary>
        /// 根据用户Ids集合获取角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetByIds(string ids)
        {
            var data = new MessageModel<List<Role>>();
            if (!string.IsNullOrEmpty(ids))
            {
                var roleList = await _roleSvc.QueryByIds(ids.Split(","));

                data.Response = roleList;
                data.Success = roleList.Count >= 0;
                data.Msg = "获取成功";
            }

            return data;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<object> Post([FromBody] Role role)
        {
            var model = await _roleSvc.SaveRole(role);

            return new MessageModel<Role>()
            {
                Msg = "添加成功",
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
        public async Task<object> Put([FromBody] Role role)
        {
            var data = new MessageModel<string>();
            if (role != null && role.Id > 0)
            {
                data.Success = await _roleSvc.Update(role);
                if (data.Success)
                {
                    data.Msg = "更新成功";
                    data.Response = role?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var role = await _roleSvc.QueryById(id);
                role.DeleteTime = DateTime.Now;
                data.Success = await _roleSvc.Update(role);
                if (data.Success)
                {
                    data.Msg = "删除成功";
                    data.Response = role?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="falg"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> Disable(int id, bool falg)
        {
            var role = await _roleSvc.QueryById(id);
            role.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _roleSvc.Update(role)
            };

            if (data.Success)
            {
                data.Msg = falg ? "禁用成功" : "启用成功";
                data.Response = role?.Id.ToString();
            }

            return data;
        }
    }
}