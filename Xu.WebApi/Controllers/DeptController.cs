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
    /// 部门管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class DeptController : ControllerBase
    {
        private readonly IDeptSvc _deptSvc;

        public DeptController(IDeptSvc deptSvc)
        {
            _deptSvc = deptSvc;
        }

        /// <summary>
        /// 获取部门数据
        /// </summary>
        /// <param name="ids">可空</param>
        /// <param name="deptName">部门名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get(string ids, string deptName = "")
        {
            var data = await _deptSvc.Query();

            if (!string.IsNullOrEmpty(ids))
                data = data.Where(a => ids.SplitInt(",").Contains(a.Id)).ToList();

            if (!string.IsNullOrEmpty(deptName))
                data = data.Where(a => a.DeptName != null && a.DeptName.Contains(deptName)).ToList();

            return new MessageModel<List<Dept>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 获取全部部门并分页
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="deptName">部门名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetByPage(int page = 1, int pageSize = 50, string deptName = "")
        {
            var data = await _deptSvc.QueryPage(a => (a.DeptName != null && a.DeptName.Contains(deptName)), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<Dept>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> Post([FromBody] Dept request)
        {
            var data = new MessageModel<string>();

            var id = await _deptSvc.Add(request);
            data.Success = id > 0;

            if (data.Success)
            {
                data.Response = id.ToString();
                data.Message = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新部门
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> Put([FromBody] Dept dept)
        {
            var data = new MessageModel<string>();
            if (dept != null && dept.Id > 0)
            {
                data.Success = await _deptSvc.Update(dept);
                if (data.Success)
                {
                    data.Message = "更新成功";
                    data.Response = dept?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id">非空</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var dept = await _deptSvc.QueryById(id);
                dept.DeleteTime = DateTime.Now;
                data.Success = await _deptSvc.Update(dept);
                if (data.Success)
                {
                    data.Message = "删除成功";
                    data.Response = dept?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用部门
        /// </summary>
        /// <param name="id">非空</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> Disable(int id, bool falg)
        {
            var dept = await _deptSvc.QueryById(id);
            dept.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _deptSvc.Update(dept)
            };

            if (data.Success)
            {
                data.Message = falg ? "禁用成功" : "启用成功";
                data.Response = dept?.Id.ToString();
            }

            return data;
        }
    }
}