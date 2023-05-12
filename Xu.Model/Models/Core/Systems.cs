using SqlSugar;

namespace Xu.Model.Models
{
    /// <summary>
    /// 系统信息表
    /// </summary>
    [SugarTable("Systems", "系统信息表")]
    [TenantAttribute("WMBLOG_MYSQL_1")]
    public class Systems : ModelBase
    {
        /// <summary>
        /// 系统简码
        /// </summary>
        [SugarColumn(ColumnDescription = "系统简码")]
        public string SystemCode { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        [SugarColumn(ColumnDescription = "系统名称")]
        public string SystemName { get; set; }

        /// <summary>
        /// 加载序号
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "加载序号")]
        public int Index { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [SugarColumn(ColumnDescription = "true 禁用，false 可用")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue)]
        public string Remark { get; set; }
    }
}