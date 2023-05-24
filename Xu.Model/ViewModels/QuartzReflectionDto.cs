namespace Xu.Model.ViewModels
{
    /// <summary>
    /// 实现IJob的类
    /// </summary>
    public class QuartzReflectionDto
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public string nameSpace { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string nameClass { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }
}