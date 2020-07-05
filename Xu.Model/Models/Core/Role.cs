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
        public string RoleCode { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 菜单Ids(三级菜单)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string MenuIds { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }
    }
}