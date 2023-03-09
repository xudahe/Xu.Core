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
using Xu.Model.XmlModels;
using Xu.Services;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 平台管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class PlatformController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformSvc _platformSvc;
        private readonly ISystemSvc _systemSvc;

        public PlatformController(IMapper mapper, IPlatformSvc platformSvc, ISystemSvc systemSvc)
        {
            _mapper = mapper;
            _platformSvc = platformSvc;
            _systemSvc = systemSvc;
        }

        /// <summary>
        /// 获取平台数据
        /// </summary>
        /// <param name="ids">平台id或guid集合（可空）</param>
        /// <param name="key">平台名称或平台简码（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetPlatform(string ids = "", string key = "")
        {
            var data = await _platformSvc.GetDataByids(ids);

            if (!string.IsNullOrEmpty(key))
                data = data.Where(a => a.PlatformName.Contains(key) || a.PlatformCode.Contains(key)).OrderBy(s => s.Index).ToList();

            return new MessageModel<List<Platform>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 获取全部平台并分页
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="deptName">平台名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetPlatformByPage(int page = 1, int pageSize = 50, string deptName = "")
        {
            var data = await _platformSvc.QueryPage(a => (a.PlatformName != null && a.PlatformName.Contains(deptName)), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<Platform>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 添加平台
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> PostPlatform([FromBody] Platform model)
        {
            var data = new MessageModel<Platform>() { Message = "添加成功", Success = true };

            var dataList = await _platformSvc.Query(a => a.PlatformName == model.PlatformName);
            if (dataList.Count > 0)
            {
                data.Message = "该平台名称已存在";
            }
            else
            {
                model.Id = await _platformSvc.Add(model);
                data.Response = model;
                data.Message = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新平台
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PutPlatform([FromBody] Platform model)
        {
            var data = new MessageModel<string>();

            var dataList = await _systemSvc.GetDataByids(model.SystemIds);
            model.SystemInfoList = _mapper.Map<IList<Systems>, IList<InfoSystem>>(dataList);

            if (model != null && model.Id > 0)
            {
                data.Success = await _platformSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "更新成功";
                }
            }

            return data;
        }

        /// <summary>
        /// 删除平台
        /// </summary>
        /// <param name="id">平台id（非空）</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeletePlatform(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var model = await _platformSvc.QueryById(id);
                model.DeleteTime = DateTime.Now;
                data.Success = await _platformSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "删除成功";
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用平台
        /// </summary>
        /// <param name="id">平台id（非空）</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DisablePlatform(int id, bool falg)
        {
            var model = await _platformSvc.QueryById(id);
            model.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _platformSvc.Update(model)
            };

            if (data.Success)
            {
                data.Message = falg ? "禁用成功" : "启用成功";
            }

            return data;
        }

        /// <summary>
        /// 平台-->系统
        /// </summary>
        /// <param name="platId">用户id或guid</param>
        /// <param name="systemId">角色id或guid，小写逗号隔开","</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PlatformBysystemId(string platId, string systemId)
        {
            var data = new MessageModel<string>();
            var platInfo = (await _platformSvc.GetDataByids(platId)).First();

            if (platInfo != null && platInfo.Id > 0)
            {
                var systemList = await _systemSvc.GetDataByids(systemId);
                platInfo.SystemInfoList = _mapper.Map<IList<Systems>, IList<InfoSystem>>(systemList);
                platInfo.SystemIds = systemId;

                data.Success = await _systemSvc.Update(platInfo);
                if (data.Success)
                {
                    data.Message = "更新成功";
                }
            }
            else
            {
                data.Message = "该平台不存在，请联系管理员";
            }

            return data;
        }
    }
}