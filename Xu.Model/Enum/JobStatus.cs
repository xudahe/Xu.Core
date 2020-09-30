using Xu.Common;

namespace Xu.Model.Enum
{
    /// <summary>
    /// Job运行状态
    /// </summary>
    public enum JobStatus
    {
        [EnumText("未启动")]
        未启动 = 1,

        [EnumText("运行中")]
        运行中 = 2,

        [EnumText("已停止")]
        已停止 = 3,
    }
}