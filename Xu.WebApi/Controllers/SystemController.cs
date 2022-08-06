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
    /// 系统管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class SystemController : ControllerBase
    {
        private readonly ISystemSvc _systemSvc;

        public SystemController(ISystemSvc systemSvc)
        {
            _systemSvc = systemSvc;
        }

        /// <summary>
        /// 获取系统数据
        /// </summary>
        /// <param name="ids">系统id或guid集合（可空）</param>
        /// <param name="key">系统名称或系统简码（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetSystem(string ids = "", string key = "")
        {
            var data = await _systemSvc.GetDataByids(ids);

            if (!string.IsNullOrEmpty(key))
                data = data.Where(a => a.SystemName.Contains(key) || a.SystemCode.Contains(key)).ToList();

            return new MessageModel<List<Systems>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 获取全部系统并分页
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="deptName">系统名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetSystemByPage(int page = 1, int pageSize = 50, string deptName = "")
        {
            var data = await _systemSvc.QueryPage(a => (a.SystemName != null && a.SystemName.Contains(deptName)), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<Systems>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 添加系统
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> PostSystem([FromBody] Systems model)
        {
            var data = new MessageModel<Systems>() { Message = "添加成功", Success = true };

            var dataList = await _systemSvc.Query(a => a.SystemName == model.SystemName);
            if (dataList.Count > 0)
            {
                data.Message = "该系统名称已存在";
                data.Success = false;
            }
            else
            {
                model.Id = await _systemSvc.Add(model);
                data.Response = model;
            }

            return data;
        }

        /// <summary>
        /// 更新系统
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PutSystem([FromBody] Systems model)
        {
            var data = new MessageModel<string>();
            if (model != null && model.Id > 0)
            {
                model.ModifyTime = DateTime.Now;
                data.Success = await _systemSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "更新成功";
                    data.Response = model?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除系统
        /// </summary>
        /// <param name="id">系统id（非空）</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeleteSystem(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var model = await _systemSvc.QueryById(id);
                model.DeleteTime = DateTime.Now;
                data.Success = await _systemSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "删除成功";
                    data.Response = model?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用系统
        /// </summary>
        /// <param name="id">系统id（非空）</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DisableSystem(int id, bool falg)
        {
            var model = await _systemSvc.QueryById(id);
            model.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _systemSvc.Update(model)
            };

            if (data.Success)
            {
                data.Message = falg ? "禁用成功" : "启用成功";
                data.Response = model?.Id.ToString();
            }

            return data;
        }
    }
}
