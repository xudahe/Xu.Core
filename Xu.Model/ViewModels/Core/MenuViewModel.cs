namespace Xu.Model.ViewModels
{
    public class MenuViewModel : ModelBase
    {
        /// <summary>
        /// 系统名称(大屏/应用)
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 关联的系统Id或guid
        /// </summary>
        public string SystemId { get; set; }

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
        /// 菜单路由
        /// </summary>
        public string RouterUrl { get; set; }

        /// <summary>
        /// 父级菜单Id
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 父级菜单名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 加载方式
        /// </summary>
        public string LoadWay { get; set; }

        /// <summary>
        /// 加载序号
        /// </summary>
        public int Index { get; set; }

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