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
        [SugarColumn(ColumnDescription = "任务名称")]
        public string JobName { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        [SugarColumn(ColumnDescription = "任务分组")]
        public string JobGroup { get; set; }

        /// <summary>
        /// 任务所在DLL对应的程序集名称，默认值："Xu.Tasks"
        /// </summary>
        [SugarColumn(ColumnDescription = "任务所在DLL对应的程序集名称")]
        public string AssemblyName { get; set; } = "Xu.Tasks";

        /// <summary>
        /// 任务所在类，默认值："JobQuartz"
        /// </summary>
        [SugarColumn(ColumnDescription = "任务所在类")]
        public string ClassName { get; set; } = "JobQuartz";

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue)]
        public string Remark { get; set; }

        /// <summary>
        /// 执行日志
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue, ColumnDescription = "执行日志")]
        public string TasksLog { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "开始时间")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "结束时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 重复执行次数,默认为0
        /// </summary>
        [SugarColumn(ColumnDescription = "重复执行次数")]
        public int RunTimes { get; set; }

        /// <summary>
        /// 最后执行时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "最后执行时间")]
        public DateTime? PerformTime { get; set; }

        /// <summary>
        /// 触发器类型（simple 或 cron）
        /// </summary>
        [SugarColumn(ColumnDescription = "触发器类型（simple 或 cron）")]
        public string TriggerType { get; set; }

        /// <summary>
        /// 间隔（Cron）-- 任务运行时间表达式
        /// <a href="http://cron.qqe2.com/" target="_blank">cron在线生成</a>
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "间隔（Cron）")]
        public string Cron { get; set; }

        /// <summary>
        /// 执行间隔时间, 秒为单位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "执行间隔时间, 秒为单位")]
        public int? IntervalSecond { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "运行状态")]
        public JobStatus? JobStatus { get; set; } = Enum.JobStatus.未启动;

        /// <summary>
        /// 执行传参
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "执行传参")]
        public string JobParams { get; set; }
    }
}