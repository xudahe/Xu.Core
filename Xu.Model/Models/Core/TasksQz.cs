using SqlSugar;
using System;
using Xu.Model.Enum;

namespace Xu.Model.Models
{
    /// <summary>
    /// 定时任务计划列表
    /// </summary>
    public class TasksQz : ModelBase
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        public string JobGroup { get; set; }

        /// <summary>
        /// 任务所在DLL对应的程序集名称，默认值："Xu.Tasks"
        /// </summary>
        public string AssemblyName { get; set; } = "Xu.Tasks";

        /// <summary>
        /// 任务所在类，默认值："JobQuartz"
        /// </summary>
        public string ClassName { get; set; } = "JobQuartz";

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 执行日志
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string TasksLog { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 重复执行次数,默认为0
        /// </summary>
        public int RunTimes { get; set; }

        /// <summary>
        /// 最后执行时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? PerformTime { get; set; }

        /// <summary>
        /// 触发器类型（simple 或 cron）
        /// </summary>
        public string TriggerType { get; set; }

        /// <summary>
        /// 间隔（Cron）-- 任务运行时间表达式
        /// <a href="http://cron.qqe2.com/" target="_blank">cron在线生成</a>
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Cron { get; set; }

        /// <summary>
        /// 执行间隔时间, 秒为单位
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int IntervalSecond { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public JobStatus? JobStatus { get; set; }

        /// <summary>
        /// 执行传参
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string JobParams { get; set; }
    }
}