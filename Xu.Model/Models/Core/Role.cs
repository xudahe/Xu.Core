using SqlSugar;

namespace Xu.Model.Models
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class Role : ModelBase
    {
        /// <summary>
        /// 角色编码
        /// </summary>
        [SugarColumn(ColumnDescription = "角色编码")]
        public string RoleCode { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [SugarColumn(ColumnDescription = "角色名称")]
        public string RoleName { get; set; }

        /// <summary>
        /// 关联菜单Ids
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue, ColumnDescription = "关联菜单Id")]
        public string MenuIds { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue)]
        public string Remark { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }
    }
}