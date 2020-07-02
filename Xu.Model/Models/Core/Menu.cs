namespace Xu.Model
{
    /// <summary>
    /// 菜单配置表
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
        public string Icon { get; set; }

        /// <summary>
        /// 父级菜单
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 加载方式
        /// </summary>
        public string LoadWay { get; set; }

        /// <summary>
        /// 加载序號
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}