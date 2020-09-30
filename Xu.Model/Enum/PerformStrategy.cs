using Xu.Common;

namespace Xu.Model.Enum
{
    /// <summary>
    /// 执行策略
    /// </summary>
    public enum PerformStrategy
    {
        [EnumText("立即执行")]
        立即执行 = 1,

        [EnumText("执行一次")]
        执行一次 = 2,

        [EnumText("放弃执行")]
        放弃执行 = 3,
    }
}