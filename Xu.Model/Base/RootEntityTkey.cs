using SqlSugar;

namespace Xu.Model
{
    /// <summary>
    /// 主键
    /// </summary>
    public class RootEntityTkey
    {
        /// <summary>
        /// 主键Id，领域对象标识
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "主键Id")]
        public int Id { get; set; }

        /// <summary>
        /// Guid
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "Guid")]
        public string Guid { get; set; }
    }
}
