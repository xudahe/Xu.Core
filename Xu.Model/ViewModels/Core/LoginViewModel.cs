using System;
using System.Collections.Generic;
using Xu.Model.Models;

namespace Xu.Model.ViewModels
{
    /// <summary>
    /// 登录接口返回的用户信息
    /// </summary>
    public class LoginViewModel : ModelBase
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
        public string RealName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int? Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birth { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        public IList<DeptViewModel> DeptInfoList { get; set; }

        public IList<RoleViewModel> RoleInfoList { get; set; }

        public IList<MenuViewModel> MenuInfoList { get; set; }
    }
}