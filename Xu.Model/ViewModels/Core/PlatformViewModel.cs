namespace Xu.Model.ViewModels
{
    public class PlatformViewModel : ModelBase
    {
        /// <summary>
        /// 平台简码
        /// </summary>
        public string PlatformCode { get; set; }

        /// <summary>
        /// 平台名称
        /// </summary>
        public string PlatformName { get; set; }

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