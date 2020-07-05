using SqlSugar;

namespace Xu.Model.Models
{
    /// <summary>
    /// 部门管理
    /// </summary>
    public class Dept : ModelBase
    {
        /// <summary>
        /// 部门编码
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string DeptManager { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ParentDept { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Remark { get; set; }
    }
}