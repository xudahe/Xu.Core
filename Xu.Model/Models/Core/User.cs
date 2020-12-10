using SqlSugar;
using System;

namespace Xu.Model.Models
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    //[SugarTable("dbo.User")]    //对应数据库的dbo.User表
    public class User : ModelBase
    {
        /// <summary>
        /// 登录账号(用户名)
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "真实姓名")]
        public string RealName { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "所属部门")]
        public string DeptName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue)]
        public string Remark { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "最后登录时间")]
        public DateTime? LastErrTime { get; set; }

        /// <summary>
        /// 登录错误次数
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "登录错误次数")]
        public int? ErrorCount { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? Birth { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Address { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 角色ids
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue, ColumnDescription = "关联角色Id")]
        public string RoleIds { get; set; }
    }
}