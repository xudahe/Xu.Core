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
        /// <param name="ids">部门id或guid集合（可空）</param>
        /// <param name="deptName">部门名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetDept(string ids = "", string deptName = "")
        {
            var data = await _deptSvc.Query();
            var deptList = await _deptSvc.GetDataByids(ids, data);

            for (int i = 0; i < deptList.Count; i++)
            {
                deptList[i].ParentName = deptList[i].ParentId.HasValue ? (data.FirstOrDefault(s => s.Id == deptList[i].ParentId)?.DeptName) : "";
            }

            if (!string.IsNullOrEmpty(deptName))

                deptList = deptList.Where(a => a.DeptName.Contains(deptName)).ToList();

            return new MessageModel<List<Dept>>()
            {
                Message = "获取成功",
                Success = true,
                Response = deptList
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
        public async Task<object> GetDeptByPage(int page = 1, int pageSize = 50, string deptName = "")
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
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> PostDept([FromBody] Dept dept)
        {
            var data = new MessageModel<Dept>() { Message = "添加成功", Success = true };

            var dataList = await _deptSvc.Query(a => a.DeptName == dept.DeptName);
            if (dataList.Count > 0)
            {
                data.Message = "该部门名称已存在";
                data.Success = false;
            }
            else
            {
                dept.Id = await _deptSvc.Add(dept);
                data.Response = dept;
            }

            return data;
        }

        /// <summary>
        /// 更新部门
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PutDept([FromBody] Dept dept)
        {
            var data = new MessageModel<string>();
            if (dept != null && dept.Id > 0)
            {
                dept.ModifyTime = DateTime.Now;
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
        /// <param name="id">部门id（非空）</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeleteDept(int id)
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
        /// <param name="id">部门id（非空）</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DisableDept(int id, bool falg)
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