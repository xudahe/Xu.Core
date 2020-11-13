using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    //[Authorize(Permissions.Name)]
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
        /// <param name="ids">可空</param>
        /// <param name="jobName">任务名称(可空)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetTasksQz(string ids, string jobName = "")
        {
            var data = await _tasksQzSvc.Query();

            if (!string.IsNullOrEmpty(ids))
                data = data.Where(a => ids.SplitInt(",").Contains(a.Id)).ToList();

            if (!string.IsNullOrEmpty(jobName))
                data = data.Where(a => a.JobName != null && a.JobName.Contains(jobName)).ToList();

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
        /// <param name="jobName">任务名称</param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetTasksQzByPage(int page = 1, int pageSize = 50, string jobName = "")
        {
            Expression<Func<TasksQz, bool>> whereExpression = a => (a.JobName != null && a.JobName.Contains(jobName));

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
        public async Task<object> PostTasksQz([FromBody] TasksQz tasksQz)
        {
            var data = new MessageModel<string>();

            tasksQz.AssemblyName = "Xu.Tasks";
            tasksQz.ClassName = "JobQuartz";
            tasksQz.JobStatus = JobStatus.未启动;
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
        public async Task<object> PutTasksQz([FromBody] TasksQz tasksQz)
        {
            var data = new MessageModel<string>();
            if (tasksQz != null && tasksQz.Id > 0)
            {
                tasksQz.AssemblyName = "Xu.Tasks";
                tasksQz.ClassName = "JobQuartz";
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
        /// <param name="id">任务Id（非空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> StartJob(int id)
        {
            var data = new MessageModel<string>();

            var model = await _tasksQzSvc.QueryById(id);
            if (model != null)
            {
                var resuleModel = await _schedulerCenter.AddScheduleJobAsync(model);
                if (resuleModel.Success)
                {
                    model.JobStatus = JobStatus.运行中;
                    data.Success = await _tasksQzSvc.Update(model);

                    if (data.Success)
                    {
                        data.Message = "启动成功";
                        data.Response = id.ToString();
                    }
                }
                else
                {
                    return resuleModel;
                }
            }
            return data;
        }

        /// <summary>
        /// 停止一个计划任务
        /// </summary>
        /// <param name="id">任务Id（非空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> StopJob(int id)
        {
            var data = new MessageModel<string>();

            var model = await _tasksQzSvc.QueryById(id);
            if (model != null)
            {
                var resuleModel = await _schedulerCenter.StopScheduleJobAsync(model);
                if (resuleModel.Success)
                {
                    model.JobStatus = JobStatus.已停止;
                    data.Success = await _tasksQzSvc.Update(model);

                    if (data.Success)
                    {
                        data.Message = "停止成功";
                        data.Response = id.ToString();
                    }
                }
                else
                {
                    return resuleModel;
                }
            }
            return data;
        }

        /// <summary>
        /// 重启一个计划任务
        /// </summary>
        /// <param name="id">任务Id（非空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> ReCovery(int id)
        {
            var data = new MessageModel<string>();

            var model = await _tasksQzSvc.QueryById(id);
            if (model != null)
            {
                var resuleModel = await _schedulerCenter.ResumeScheduleJobAsync(model);
                if (resuleModel.Success)
                {
                    model.JobStatus = JobStatus.运行中;
                    data.Success = await _tasksQzSvc.Update(model);

                    if (data.Success)
                    {
                        data.Message = "重启成功";
                        data.Response = id.ToString();
                    }
                }
                else
                {
                    return resuleModel;
                }
            }
            return data;
        }

        /// <summary>
        /// 删除一个计划任务
        /// </summary>
        /// <param name="id">非空</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeleteTasksQz(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var tasks = await _tasksQzSvc.QueryById(id);
                tasks.DeleteTime = DateTime.Now;
                data.Success = await _tasksQzSvc.Update(tasks);
                if (data.Success)
                {
                    data.Message = "删除成功";
                    data.Response = tasks?.Id.ToString();
                }
            }

            return data;
        }
    }
}