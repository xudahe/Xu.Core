using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xu.Model.Models
{
    /// <summary>
    /// 系统管理
    /// </summary>
    [SugarTable("Systems", "WMBLOG_MYSQL")]
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
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue)]
        public string Remark { get; set; }
    }
}
