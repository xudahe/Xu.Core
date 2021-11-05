namespace Xu.Model
{
    /// <summary>
    /// 所需分页参数
    /// </summary>
    public class PaginationModel
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int IntPageIndex { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int IntPageSize { get; set; } = 10;

        /// <summary>
        /// 排序字段(例如:id desc,time asc)
        /// </summary>
        public string StrOrderByFileds { get; set; }

        /// <summary>
        /// 查询条件( 例如:id = 1 and name = 小明)
        /// </summary>
        public string Conditions { get; set; }
    }
}