using SqlSugar;
using System;

namespace Xu.Model.Models
{
    /// <summary>
    /// 定时任务执行日志表
    /// </summary>
    public class TasksLog : ModelBase
    {
        /// <summary>
        /// 执行任务Id
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "执行任务Id")]
        public int PerformJobId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "任务名称")]
        public string JobName { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "任务分组")]
        public string JobGroup { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "执行时间")]
        public DateTime PerformTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "备注")]
        public string Remark { get; set; }
    }
}