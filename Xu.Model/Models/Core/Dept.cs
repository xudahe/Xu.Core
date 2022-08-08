using SqlSugar;

namespace Xu.Model.Models
{
    /// <summary>
    /// 部门管理
    /// </summary>
    [SugarTable("Dept", "WMBLOG_MYSQL_1")]
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
        /// 上级部门
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "上级部门Id")]
        public int? ParentId { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [SugarColumn(ColumnDescription = "true 禁用，false 可用")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue)]
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