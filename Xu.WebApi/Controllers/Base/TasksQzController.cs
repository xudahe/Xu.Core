using Microsoft.AspNetCore.Mvc;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Xu.Common;
using Xu.EnumHelper;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Model.ResultModel;
using Xu.Model.ViewModels;
using Xu.Tasks;
using Xu.Repository;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 任务调度
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize(Permissions.Name)]
    public class TasksQzController : ControllerBase
    {
        private readonly ITasksQzSvc _tasksQzSvc;
        private readonly ISchedulerCenter _schedulerCenter;
        private readonly IUnitOfWorkManage _unitOfWorkManage;

        public TasksQzController(ITasksQzSvc tasksQzSvc, ISchedulerCenter schedulerCenter, IUnitOfWorkManage unitOfWorkManage)
        {
            _unitOfWorkManage = unitOfWorkManage;
            _tasksQzSvc = tasksQzSvc;
            _schedulerCenter = schedulerCenter;
        }

        /// <summary>
        /// 获取全部任务
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

            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    item.Triggers = await _schedulerCenter.GetTaskStaus(item);
                }
            }

            return new MessageModel<List<TasksQz>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 获取全部任务并分页
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

            _unitOfWorkManage.BeginTran();

            tasksQz.JobStatus = tasksQz.IsStart ? JobStatus.运行中 : JobStatus.未启动;
            var id = await _tasksQzSvc.Add(tasksQz);
            data.Success = id > 0;
            data.Response = id.ToString();
            data.Message = data.Success ? "添加成功" : "添加失败";

            try
            {
                if (data.Success)
                {
                    tasksQz.Id = id;
                    if (tasksQz.IsStart)
                    {
                        var resuleModel = await _schedulerCenter.AddScheduleJobAsync(tasksQz);
                        data.Success = resuleModel.Success;
                        if (resuleModel.Success)
                        {
                            data.Message = $"{data.Message}=>启动成功=>{resuleModel.Message}";
                        }
                        else
                        {
                            data.Message = $"{data.Message}=>启动失败=>{resuleModel.Message}";
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (data.Success)
                    _unitOfWorkManage.CommitTran();
                else
                    _unitOfWorkManage.RollbackTran();
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
                _unitOfWorkManage.BeginTran();

                tasksQz.JobStatus = tasksQz.JobStatus;
                data.Success = await _tasksQzSvc.Update(tasksQz);
                data.Response = tasksQz.Id.ToString();
                data.Message = data.Success ? "更新成功" : "更新失败";

                try
                {
                    if (data.Success)
                    {
                        if (tasksQz.JobStatus == JobStatus.运行中)
                        {
                            var resuleModelStop = await _schedulerCenter.StopScheduleJobAsync(tasksQz);
                            data.Success = resuleModelStop.Success;
                            data.Message = $"{data.Message}=>停止:{resuleModelStop.Message}";

                            var resuleModelStar = await _schedulerCenter.AddScheduleJobAsync(tasksQz);
                            data.Success = resuleModelStar.Success;
                            data.Message = $"{data.Message}=>启动:{resuleModelStar.Message}";
                        }
                        else
                        {
                            var resuleModelStop = await _schedulerCenter.StopScheduleJobAsync(tasksQz);
                            data.Success = resuleModelStop.Success;
                            data.Message = $"{data.Message}=>停止:{resuleModelStop.Message}";
                        }
                    }
                    else
                    {
                        data.Message = "任务不存在";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.Success)
                        _unitOfWorkManage.CommitTran();
                    else
                        _unitOfWorkManage.RollbackTran();
                }
            }
            return data;
        }

        /// <summary>
        /// 删除计划任务
        /// </summary>
        /// <param name="id">非空</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeleteTasksQz(int id)
        {
            var data = new MessageModel<string>();

            var model = await _tasksQzSvc.QueryById(id);
            if (model != null)
            {
                _unitOfWorkManage.BeginTran();

                model.DeleteTime = DateTime.Now;
                model.JobStatus = JobStatus.已停止;
                data.Success = await _tasksQzSvc.Update(model);
                data.Response = model.Id.ToString();
                data.Message = data.Success ? "更新成功" : "更新失败";

                try
                {
                    if (data.Success)
                    {
                        var resuleModel = await _schedulerCenter.DeleteScheduleJobAsync(model);
                        data.Success = resuleModel.Success;
                        if (resuleModel.Success)
                        {
                            data.Message = $"{data.Message}=>删除成功=>{resuleModel.Message}";
                        }
                        else
                        {
                            data.Message = $"{data.Message}=>删除失败=>{resuleModel.Message}";
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.Success)
                        _unitOfWorkManage.CommitTran();
                    else
                        _unitOfWorkManage.RollbackTran();
                }
            }
            else
            {
                data.Message = "任务不存在";
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
                model.JobStatus = JobStatus.运行中;
                data.Success = await _tasksQzSvc.Update(model);
                data.Response = id.ToString();
                data.Message = data.Success ? "更新成功" : "更新失败";

                if (data.Success)
                {
                    var resuleModel = await _schedulerCenter.AddScheduleJobAsync(model);
                    data.Success = resuleModel.Success;
                    if (resuleModel.Success)
                    {
                        data.Message = $"{data.Message}=>启动成功=>{resuleModel.Message}";
                    }
                    else
                    {
                        data.Message = $"{data.Message}=>启动失败=>{resuleModel.Message}";
                    }
                }
            }
            else
            {
                data.Message = "任务不存在";
            }
            return data;
        }

        /// <summary>
        /// 停止计划任务
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
                model.JobStatus = JobStatus.已停止;
                data.Success = await _tasksQzSvc.Update(model);
                data.Response = id.ToString();
                data.Message = data.Success ? "更新成功" : "更新失败";
                if (data.Success)
                {
                    var resuleModel = await _schedulerCenter.StopScheduleJobAsync(model);
                    data.Success = resuleModel.Success;
                    if (resuleModel.Success)
                    {
                        data.Message = $"{data.Message}=>停止成功=>{resuleModel.Message}";
                    }
                    else
                    {
                        data.Message = $"{data.Message}=>停止失败=>{resuleModel.Message}";
                    }
                }
            }
            else
            {
                data.Message = "任务不存在";
            }
            return data;
        }

        /// <summary>
        /// 重启计划任务
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
                model.JobStatus = JobStatus.运行中;
                data.Success = await _tasksQzSvc.Update(model);
                data.Response = id.ToString();
                data.Message = data.Success ? "更新成功" : "更新失败";
                if (data.Success)
                {
                    var resuleModel = await _schedulerCenter.ResumeScheduleJobAsync(model);
                    data.Success = resuleModel.Success;
                    if (resuleModel.Success)
                    {
                        data.Message = $"{data.Message}=>重启成功=>{resuleModel.Message}";
                    }
                    else
                    {
                        data.Message = $"{data.Message}=>重启失败=>{resuleModel.Message}";
                    }
                }
            }
            else
            {
                data.Message = "任务不存在";
            }
            return data;
        }

        /// <summary>
        /// 获取任务命名空间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<List<QuartzReflectionViewModel>> GetTaskNameSpace()
        {
            var baseType = typeof(IJob);
            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var referencedAssemblies = System.IO.Directory.GetFiles(path, "Xu.Tasks.dll").Select(Assembly.LoadFrom).ToArray();
            var types = referencedAssemblies
                .SelectMany(a => a.DefinedTypes)
                .Select(type => type.AsType())
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
            var implementTypes = types.Where(x => x.IsClass).Select(item => new QuartzReflectionViewModel { nameSpace = item.Namespace, nameClass = item.Name, remark = "" }).ToList();
            return MessageModel<List<QuartzReflectionViewModel>>.Msg(true, "获取成功", implementTypes);
        }
    }
}