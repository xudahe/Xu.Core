using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xu.Common;

namespace Xu.Model.Models
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class Role : ModelBase
    {
        /// <summary>
        /// 角色简码
        /// </summary>
        [SugarColumn(ColumnDescription = "角色简码")]
        public string RoleCode { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [SugarColumn(ColumnDescription = "角色名称")]
        public string RoleName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue)]
        public string Remark { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 角色关联菜单的id或guid集合，不能同时包含两者
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue, ColumnDescription = "角色关联菜单的id或guid集合")]
        public string MenuIds { get; set; }

        #region 绑定菜单

        private string _menuInfoItem;

        [SugarColumn(IsIgnore = true)]
        private IList<InfoMenu> _menuInfoList { get; set; }

        /// <summary>
        /// 关联菜单List
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public IList<InfoMenu> MenuInfoList
        {
            get { return _menuInfoList; }
            set
            {
                _menuInfoList = value;
                if (_menuInfoList != null && _menuInfoList.Count > 0)
                    _menuInfoItem = ToItemInfoXml();
                else
                    _menuInfoItem = null;
            }
        }

        /// <summary>
        /// 关联菜单Xml
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue, ColumnDescription = "关联菜单")]
        public string MenuInfoXml
        {
            get { return _menuInfoItem; }
            set
            {
                _menuInfoItem = value;
                if (!string.IsNullOrEmpty(_menuInfoItem))
                    _menuInfoList = FromItemInfoXml();
            }
        }

        public string ToItemInfoXml()
        {
            XElement xElement = new XElement("Menu");
            xElement.SetAttributeValue("Version", "1");

            if (_menuInfoList.Count > 0)
            {
                foreach (var item in _menuInfoList)
                {
                    XElement xItem = new XElement("InfoItem");
                    xItem.SetAttributeValue("Id", item.Id);
                    xItem.SetAttributeValue("Guid", item.Guid);
                    xItem.SetAttributeValue("MenuName", item.MenuName);
                    xElement.Add(xItem);
                }
                return xElement.ToString();
            }
            return null;
        }

        public IList<InfoMenu> FromItemInfoXml()
        {
            IList<InfoMenu> list = new List<InfoMenu>();
            XElement x = XElement.Parse(_menuInfoItem);
            if (x.Name != "Menu")
                return list;
            XAttribute ver = x.Attribute("Version");
            if (ver == null || ver.Value != "1")
                return list;
            IList<XElement> xitems = x.Descendants("InfoItem").ToList();
            if (xitems.Count > 0)
            {
                foreach (var xElement in xitems)
                {
                    InfoMenu item = new InfoMenu
                    {
                        Id = xElement.Attribute("Id").Value.ToInt32Req(),
                        Guid = xElement.Attribute("Guid").Value,
                        MenuName = xElement.Attribute("MenuName").Value,
                    };
                    list.Add(item);
                }
                return list;
            }
            return null;
        }

        #endregion 绑定菜单
    }
}