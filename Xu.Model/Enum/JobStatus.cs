using Xu.Common;

namespace Xu.Model.Enum
{
    /// <summary>
    /// Job运行状态
    /// </summary>
    public enum JobStatus
    {
        [EnumText("初始化")]
        初始化 = 0,

        [EnumText("运行中")]
        运行中 = 1,

        [EnumText("已停止")]
        已停止 = 2,
    }
}