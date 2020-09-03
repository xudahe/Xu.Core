using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model.Enum;
using Xu.Model.Models;
using Xu.Model.ResultModel;
using Xu.Tasks;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 定时任务
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class TasksQzController : ControllerBase
    {
        private readonly ITasksQzSvc _tasksQzSvc;
        private readonly ISchedulerCenter _schedulerCenter;

        public TasksQzController(ITasksQzSvc tasksQzSvc, ISchedulerCenter schedulerCenter)
        {
            _tasksQzSvc = tasksQzSvc;
            _schedulerCenter = schedulerCenter;
        }

        /// <summary>
        /// 获取全部定时任务
        /// </summary>
        /// <param name="key">任务名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get(string key = "")
        {
            Expression<Func<TasksQz, bool>> whereExpression = a => a.Enabled != true && (a.JobName != null && a.JobName.Contains(key));

            var data = await _tasksQzSvc.Query();

            return new MessageModel<List<TasksQz>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 获取全部定时任务并分页
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="key">任务名称</param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetByPage(int page = 1, int pageSize = 50, string key = "")
        {
            Expression<Func<TasksQz, bool>> whereExpression = a => a.Enabled != true && (a.JobName != null && a.JobName.Contains(key));

            var data = await _tasksQzSvc.QueryPage(whereExpression, page, pageSize, " Id desc ");

            return new MessageModel<PageModel<TasksQz>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 添加计划任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> Post([FromBody] TasksQz tasksQz)
        {
            var data = new MessageModel<string>();

            tasksQz.JobStatus = JobStatus.初始化;
            var id = await _tasksQzSvc.Add(tasksQz);
            data.Success = id > 0;
            if (data.Success)
            {
                data.Response = id.ToString();
                data.Message = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新计划任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> Put([FromBody] TasksQz tasksQz)
        {
            var data = new MessageModel<string>();
            if (tasksQz != null && tasksQz.Id > 0)
            {
                data.Success = await _tasksQzSvc.Update(tasksQz);
                if (data.Success)
                {
                    data.Message = "更新成功";
                    data.Response = tasksQz?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 启动计划任务
        /// </summary>
        /// <param name="jobId">任务Id（非空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> StartJob(int jobId)
        {
            var data = new MessageModel<string>();

            var model = await _tasksQzSvc.QueryById(jobId);
            if (model != null)
            {
                var ResuleModel = await _schedulerCenter.AddScheduleJobAsync(model);
                if (ResuleModel.Success)
                {
                    model.JobStatus = JobStatus.运行中;
                    data.Success = await _tasksQzSvc.Update(model);
                }
                if (data.Success)
                {
                    data.Message = "启动成功";
                    data.Response = jobId.ToString();
                }
            }
            return data;
        }

        /// <summary>
        /// 停止一个计划任务
        /// </summary>
        /// <param name="jobId">任务Id（非空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> StopJob(int jobId)
        {
            var data = new MessageModel<string>();

            var model = await _tasksQzSvc.QueryById(jobId);
            if (model != null)
            {
                var ResuleModel = await _schedulerCenter.StopScheduleJobAsync(model);
                if (ResuleModel.Success)
                {
                    model.JobStatus = JobStatus.已停止;
                    data.Success = await _tasksQzSvc.Update(model);
                }
                if (data.Success)
                {
                    data.Message = "暂停成功";
                    data.Response = jobId.ToString();
                }
            }
            return data;
        }

        /// <summary>
        /// 重启一个计划任务
        /// </summary>
        /// <param name="jobId">任务Id（非空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> ReCovery(int jobId)
        {
            var data = new MessageModel<string>();

            var model = await _tasksQzSvc.QueryById(jobId);
            if (model != null)
            {
                var ResuleModel = await _schedulerCenter.ResumeScheduleJobAsync(model);
                if (ResuleModel.Success)
                {
                    model.JobStatus = JobStatus.运行中;
                    data.Success = await _tasksQzSvc.Update(model);
                }
                if (data.Success)
                {
                    data.Message = "重启成功";
                    data.Response = jobId.ToString();
                }
            }
            return data;
        }
    }
}