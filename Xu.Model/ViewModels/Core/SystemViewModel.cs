namespace Xu.Model.ViewModels
{
    public class SystemViewModel : ModelBase
    {
        /// <summary>
        /// 系统简码
        /// </summary>
        public string SystemCode { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

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