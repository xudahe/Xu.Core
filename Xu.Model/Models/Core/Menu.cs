﻿using SqlSugar;
using System.Collections.Generic;

namespace Xu.Model.Models
{
    /// <summary>
    /// 菜单信息表（目前菜单共三级）
    /// </summary>
    [SugarTable("Menu", "菜单信息表")]
    [TenantAttribute("WMBLOG_MYSQL_1")]
    public class Menu : ModelBase
    {
        /// <summary>
        /// 关联的系统Id或guid
        /// </summary>
        [SugarColumn(ColumnDescription = "系统Id")]
        public string SystemId { get; set; }

        /// <summary>
        /// 菜单类名
        /// </summary>
        [SugarColumn(ColumnDescription = "菜单类名")]
        public string ClassName { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [SugarColumn(ColumnDescription = "菜单名称")]
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [SugarColumn(ColumnDescription = "菜单图标")]
        public string Icon { get; set; }

        /// <summary>
        /// 菜单路由
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "菜单路由")]
        public string RouterUrl { get; set; }

        /// <summary>
        /// 父级菜单Id或guid
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "父级菜单Id或guid")]
        public string ParentId { get; set; }

        /// <summary>
        /// 加载方式
        /// </summary>
        [SugarColumn(ColumnDescription = "加载方式")]
        public string LoadWay { get; set; }

        /// <summary>
        /// 加载序号
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "加载序号")]
        public int Index { get; set; }

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
        /// 父级菜单名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string ParentName { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string SystemName { get; set; }

        /// <summary>
        /// 子级菜单
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<Menu> Children { get; set; }

        #endregion 临时类
    }
}