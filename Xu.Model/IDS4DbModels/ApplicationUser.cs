using SqlSugar;
using System;

namespace Xu.Model.IDS4DbModels
{
    /// <summary>
    /// 以下model 来自ids4项目，多库模式，为了调取ids4数据
    /// 用户表
    /// </summary>
    [SugarTable("AspNetUsers", "WMBLOG_MYSQL_2")]
    public class ApplicationUser
    {
        public string LoginName { get; set; }

        public string RealName { get; set; }

        public int Sex { get; set; } = 0;

        public int Age { get; set; }

        public DateTime Birth { get; set; } = DateTime.Now;

        public string Addr { get; set; }

        public bool TdIsDelete { get; set; }
    }
}