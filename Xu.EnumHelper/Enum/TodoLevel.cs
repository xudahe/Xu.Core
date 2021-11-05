namespace Xu.EnumHelper
{
    /// <summary>
    /// 待办紧急程度
    /// </summary>
    public enum TodoLevel
    {
        [EnumText("一般")]
        一般 = 1,

        [EnumText("急")]
        急 = 2,

        [EnumText("特急")]
        特急 = 3,
    }
}