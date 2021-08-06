using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Model.Models;
using Xu.Model.ResultModel;
using Xu.Model.ViewModel;

namespace Xu.Tasks
{
    /// <summary>
    /// 任务调度接口
    /// </summary>
    public interface ISchedulerCenter
    {
        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        Task<MessageModel<string>> StartScheduleAsync();

        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        Task<MessageModel<string>> StopScheduleAsync();

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        Task<MessageModel<string>> AddScheduleJobAsync(TasksQz tasksQz);

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        Task<MessageModel<string>> StopScheduleJobAsync(TasksQz tasksQz);

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        Task<MessageModel<string>> ResumeScheduleJobAsync(TasksQz tasksQz);

        /// <summary>
        /// 检测任务是否存在
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        Task<bool> IsExistScheduleJobAsync(TasksQz tasksQz);

        /// <summary>
        /// 获取任务触发器状态
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        Task<List<TaskInfoDto>> GetTaskStaus(TasksQz tasksQz);

        /// <summary>
        /// 获取触发器标识
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetTriggerState(string key);

        /// <summary>
        /// 立即执行 一个任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        Task<MessageModel<string>> ExecuteJobAsync(TasksQz tasksQz);
    }
}