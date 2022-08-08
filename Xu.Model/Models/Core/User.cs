using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xu.Common;

namespace Xu.Model.Models
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    [SugarTable("User", "WMBLOG_MYSQL_1")]    //可不写,对应数据库的dbo.User表
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
        public int? DeptId { get; set; }

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
        [SugarColumn(ColumnDescription = "true 禁用，false 可用")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 用户关联角色的id或guid集合，不能同时包含两者
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue, ColumnDescription = "用户关联角色的id或guid集合")]
        public string RoleIds { get; set; }

        #region 绑定角色

        private string _roleInfoItem;

        [SugarColumn(IsIgnore = true)]
        private IList<InfoRole> _roleInfoList { get; set; }

        /// <summary>
        /// 关联角色List
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public IList<InfoRole> RoleInfoList
        {
            get { return _roleInfoList; }
            set
            {
                _roleInfoList = value;
                if (_roleInfoList != null && _roleInfoList.Count > 0)
                    _roleInfoItem = ToItemInfoXml();
                else
                    _roleInfoItem = null;
            }
        }

        /// <summary>
        /// 关联角色Xml
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue, ColumnDescription = "关联角色")]
        public string RoleInfoXml
        {
            get { return _roleInfoItem; }
            set
            {
                _roleInfoItem = value;
                if (!string.IsNullOrEmpty(_roleInfoItem))
                    _roleInfoList = FromItemInfoXml();
            }
        }

        public string ToItemInfoXml()
        {
            XElement xElement = new XElement("Role");
            xElement.SetAttributeValue("Version", "1");

            if (_roleInfoList.Count > 0)
            {
                foreach (var item in _roleInfoList)
                {
                    XElement xItem = new XElement("InfoItem");
                    xItem.SetAttributeValue("Id", item.Id);
                    xItem.SetAttributeValue("Guid", item.Guid);
                    xItem.SetAttributeValue("RoleName", item.RoleName);
                    xElement.Add(xItem);
                }
                return xElement.ToString();
            }
            return null;
        }

        public IList<InfoRole> FromItemInfoXml()
        {
            IList<InfoRole> list = new List<InfoRole>();
            XElement x = XElement.Parse(_roleInfoItem);
            if (x.Name != "Role")
                return list;
            XAttribute ver = x.Attribute("Version");
            if (ver == null || ver.Value != "1")
                return list;
            IList<XElement> xitems = x.Descendants("InfoItem").ToList();
            if (xitems.Count > 0)
            {
                foreach (var xElement in xitems)
                {
                    InfoRole item = new InfoRole
                    {
                        Id = xElement.Attribute("Id").Value.ToInt32Req(),
                        Guid = xElement.Attribute("Guid").Value,
                        RoleName = xElement.Attribute("RoleName").Value,
                    };
                    list.Add(item);
                }
                return list;
            }
            return null;
        }

        #endregion 绑定角色
    }
}