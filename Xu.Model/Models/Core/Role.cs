using SqlSugar;
using SqlSugar.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xu.Model.XmlModels;

namespace Xu.Model.Models
{
    /// <summary>
    /// 角色信息表
    /// </summary>
    [SugarTable("Role", "角色信息表")]
    [TenantAttribute("WMBLOG_MYSQL_1")]
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
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue)]
        public string Remark { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [SugarColumn(ColumnDescription = "true 禁用，false 可用")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 角色关联菜单的id或guid集合，不能同时包含两者（旧：是角色绑定菜单）
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue, ColumnDescription = "角色关联菜单的id或guid集合")]
        public string MenuIds { get; set; }

        #region 绑定（新：系统-->菜单）

        private string _infoItem;

        [SugarColumn(IsIgnore = true)]
        private IList<InfoSystem> _infoList { get; set; }

        /// <summary>
        /// 关联系统List
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue, IsJson = true, ColumnDescription = "关联平台 json")]
        public IList<InfoSystem> InfoList
        {
            get { return _infoList; }
            set
            {
                _infoList = value;
                if (_infoList != null && _infoList.Count > 0)
                    _infoItem = ToItemInfo_Xml();
                else
                    _infoItem = null;
            }
        }

        /// <summary>
        /// 关联系统--菜单 Xml
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue, ColumnDescription = "关联平台--系统--菜单 xml")]
        public string InfoXml
        {
            get { return _infoItem; }
            set
            {
                _infoItem = value;
                if (!string.IsNullOrEmpty(_infoItem))
                    _infoList = FromItemInfo_Xml();
            }
        }

        public string ToItemInfo_Xml()
        {
            XElement xElement = new XElement("Xml");
            xElement.SetAttributeValue("Version", "1");

            //系统
            if (_infoList.Count > 0)
            {
                foreach (var item in _infoList)
                {
                    XElement xItem = new XElement("System");
                    xItem.SetAttributeValue("Id", item.Id);
                    xItem.SetAttributeValue("Guid", item.Guid);
                    xItem.SetAttributeValue("SystemName", item.SystemName);

                    //系统
                    if (item.InfoMenuList.Count > 0)
                    {
                        foreach (var item2 in item.InfoMenuList)
                        {
                            XElement xItem2 = new XElement("Menu");
                            xItem2.SetAttributeValue("Id", item2.Id);
                            xItem2.SetAttributeValue("Guid", item2.Guid);
                            xItem2.SetAttributeValue("MenuName", item2.MenuName);

                            xItem.Add(xItem2);
                        }
                    }

                    xElement.Add(xItem);
                }
                return xElement.ToString();
            }
            return null;
        }

        public IList<InfoSystem> FromItemInfo_Xml()
        {
            IList<InfoSystem> list = new List<InfoSystem>();
            XElement x = XElement.Parse(_infoItem);
            if (x.Name != "Xml")
                return list;
            XAttribute ver = x.Attribute("Version");
            if (ver == null || ver.Value != "1")
                return list;

            //系统
            IList<XElement> xitems = x.Descendants("System").ToList();
            if (xitems.Count > 0)
            {
                foreach (var xElement in xitems)
                {
                    InfoSystem item = new InfoSystem
                    {
                        Id = xElement.Attribute("Id").Value.ObjToInt(),
                        Guid = xElement.Attribute("Guid").Value,
                        SystemName = xElement.Attribute("SystemName").Value,
                    };

                    //菜单
                    IList<XElement> xitems2 = xElement.Descendants("Menu").ToList();
                    if (xitems2.Count > 0)
                    {
                        foreach (var xElement2 in xitems2)
                        {
                            InfoMenu item2 = new InfoMenu
                            {
                                Id = xElement2.Attribute("Id").Value.ObjToInt(),
                                Guid = xElement2.Attribute("Guid").Value,
                                MenuName = xElement2.Attribute("MenuName").Value,
                            };

                            item.InfoMenuList.Add(item2);
                        }
                    }

                    list.Add(item);
                }
                return list;
            }
            return null;
        }

        #endregion 绑定（新：系统-->菜单）
    }
}