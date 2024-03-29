﻿using SqlSugar;

namespace Xu.Model.Models
{
    /// <summary>
    /// 部门信息表
    /// </summary>
    [SugarTable("Dept", "部门信息表")]
    [TenantAttribute("WMBLOG_MYSQL_1")]
    public class Dept : ModelBase
    {
        /// <summary>
        /// 部门简码
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "部门简码")]
        public string DeptCode { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [SugarColumn(ColumnDescription = "部门名称")]
        public string DeptName { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "部门负责人")]
        public string DeptManager { get; set; }

        /// <summary>
        /// 上级部门id或guid
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "上级部门id或guid")]
        public string ParentId { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [SugarColumn(ColumnDescription = "true 禁用，false 可用")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue)]
        public string Remark { get; set; }

        #region 临时类

        /// <summary>
        /// 上级部门名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string ParentName { get; set; }

        #endregion 临时类
    }
}