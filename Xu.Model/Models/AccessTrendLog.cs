using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xu.Model.Models
{
    /// <summary>
    /// 用户访问趋势日志
    /// </summary>
    public class AccessTrendLog : ModelBase
    {
        /// <summary>
        /// 用户
        /// </summary>
        [SugarColumn(Length = 128, IsNullable = true, ColumnDataType = "nvarchar")]
        public string User { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public int Count { get; set; }
    }
}