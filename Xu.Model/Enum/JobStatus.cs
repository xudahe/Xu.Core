using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Xu.Model.Enum
{
    /// <summary>
    /// Job运行状态
    /// </summary>
    public enum JobStatus
    {
        [Description("初始化")]
        初始化 = 0,
        [Description("运行中")]
        运行中 = 1,
        [Description("已停止")]
        已停止 = 3,
    }
}
