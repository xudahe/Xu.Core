using SqlSugar;
using System.Collections.Generic;

namespace Xu.Model.Models
{
    /// <summary>
    /// 菜单配置表（目前菜单共三级）
    /// </summary>
    public class Menu : ModelBase
    {
        /// <summary>
        /// 系统名称(大屏/应用)
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 菜单类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Icon { get; set; }

        /// <summary>
        /// 菜单路由
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string RouterUrl { get; set; }

        /// <summary>
        /// 父级菜单Id
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? ParentId { get; set; }

        /// <summary>
        /// 加载方式
        /// </summary>
        public string LoadWay { get; set; }

        /// <summary>
        /// 加载序号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Index { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Remark { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<Menu> Children { get; set; }
    }
}