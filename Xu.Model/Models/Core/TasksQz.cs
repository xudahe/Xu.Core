using SqlSugar;
using System;
using System.Collections.Generic;
using Xu.EnumHelper;
using Xu.Model.ViewModels;

namespace Xu.Model.Models
{
    /// <summary>
    /// 定时任务计划列表
    /// </summary>
    [SugarTable("TasksQz", "任务调度")]
    [TenantAttribute("WMBLOG_MYSQL_1")]
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
        /// 任务所在DLL对应的程序集名称，例如："Xu.Tasks"
        /// </summary>
        [SugarColumn(ColumnDescription = "任务所在DLL对应的程序集名称")]
        public string AssemblyName { get; set; } = "Xu.Tasks";

        /// <summary>
        /// 任务所在类，例如："JobQuartz"
        /// </summary>
        [SugarColumn(ColumnDescription = "任务所在类")]
        public string ClassName { get; set; } = "JobQuartz";

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue)]
        public string Remark { get; set; }

        /// <summary>
        /// 执行日志
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue, ColumnDescription = "执行日志")]
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
        /// 循环次数(单位：次)
        /// </summary>
        [SugarColumn(ColumnDescription = "循环次数(单位：次)")]
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
        /// cron模式 -- Cron表达式
        /// <a href="http://cron.qqe2.com/" target="_blank">cron在线生成</a>
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = " Cron表达式")]
        public string Cron { get; set; }

        /// <summary>
        /// simple模式 -- 循环周期(单位：秒)
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "循环周期(单位：秒)")]
        public int IntervalSecond { get; set; }

        /// <summary>
        /// simple模式 -- 循环执行次数(单位：次)
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "循环执行次数(单位：次)")]
        public int CycleRunTimes { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "运行状态")]
        public JobStatus? JobStatus { get; set; } = EnumHelper.JobStatus.未启动;

        /// <summary>
        /// 执行传参
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "执行传参")]
        public string JobParams { get; set; }

        #region 临时类

        /// <summary>
        /// 是否立即启动
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool IsStart { get; set; } = false;

        /// <summary>
        /// 任务内存中的状态
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<TaskInfoDto> Triggers { get; set; }

        #endregion 临时类
    }
}