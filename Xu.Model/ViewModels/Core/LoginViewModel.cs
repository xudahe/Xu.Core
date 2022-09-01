using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public IList<RoleModel> RoleInfoList { get; set; }

        public IList<MenuViewModel> MenuInfoList { get; set; }

        public IList<DeptViewModel> DeptInfoList { get; set; }

        /// <summary>
        /// 返回所关联数据： 平台-->系统-->菜单
        /// </summary>
        public IList<PlatformModel> PlatformInfoList { get; set; }
    }

    /// <summary>
    /// 返回的角色信息
    /// </summary>
    public class RoleModel : ModelBase
    {
        /// <summary>
        /// 角色简码
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// 返回的平台信息
    /// </summary>
    public class PlatformModel : ModelBase
    {
        /// <summary>
        /// 平台简码
        /// </summary>
        public string PlatformCode { get; set; }

        /// <summary>
        /// 平台名称
        /// </summary>
        public string PlatformName { get; set; }

        /// <summary>
        /// 加载序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 所关联的菜单
        /// </summary>
        public IList<SystemModel> MenuInfoList { get; set; }
    }

    /// <summary>
    /// 平台所关联的系统信息
    /// </summary>
    public class SystemModel : ModelBase
    {
        /// <summary>
        /// 系统简码
        /// </summary>
        public string SystemCode { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 加载序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 所关联的菜单
        /// </summary>
        public IList<MenuModel> MenuInfoList { get; set; }
    }

    /// <summary>
    /// 系统所关联的菜单信息
    /// </summary>
    public class MenuModel : ModelBase
    {
        /// <summary>
        /// 所关联的系统Id
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 菜单类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 菜单路由
        /// </summary>
        public string RouterUrl { get; set; }

        /// <summary>
        /// 父级菜单Id或guid
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 父级菜单名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 加载方式
        /// </summary>
        public string LoadWay { get; set; }

        /// <summary>
        /// 加载序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}