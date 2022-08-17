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
    [SugarTable("Role", "WMBLOG_MYSQL_1")]
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
        [SugarColumn(ColumnDescription = "true 禁用，false 可用")]
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

        #region 绑定平台

        //private string _infoItem;

        //[SugarColumn(IsIgnore = true)]
        //private IList<InfoPlatform> _infoList { get; set; }

        ///// <summary>
        ///// 关联平台List
        ///// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public IList<InfoPlatform> InfoList
        //{
        //    get { return _infoList; }
        //    set
        //    {
        //        _infoList = value;
        //        if (_infoList != null && _infoList.Count > 0)
        //            _infoItem = ToItemInfo_Xml();
        //        else
        //            _infoItem = null;
        //    }
        //}

        ///// <summary>
        ///// 关联平台--系统--菜单 Xml
        ///// </summary>
        //[SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = int.MaxValue, ColumnDescription = "关联平台--系统--菜单 Xml")]
        //public string InfoXml
        //{
        //    get { return _infoItem; }
        //    set
        //    {
        //        _infoItem = value;
        //        if (!string.IsNullOrEmpty(_infoItem))
        //            _infoList = FromItemInfo_Xml();
        //    }
        //}

        //public string ToItemInfo_Xml()
        //{
        //    XElement xElement = new XElement("InfoItem");
        //    xElement.SetAttributeValue("Version", "1");

        //    //平台
        //    if (_infoList.Count > 0)
        //    {
        //        foreach (var item in _infoList)
        //        {
        //            XElement xItem = new XElement("Platform");
        //            xItem.SetAttributeValue("Id", item.Id);
        //            xItem.SetAttributeValue("Guid", item.Guid);
        //            xItem.SetAttributeValue("PlatformName", item.PlatformName);

        //            //系统
        //            if (item.InfoSystemList.Count > 0)
        //            {
        //                foreach (var item2 in item.InfoSystemList)
        //                {
        //                    XElement xItem2 = new XElement("System");
        //                    xItem2.SetAttributeValue("Id", item2.Id);
        //                    xItem2.SetAttributeValue("Guid", item2.Guid);
        //                    xItem2.SetAttributeValue("SystemName", item2.SystemName);

        //                    //菜单
        //                    if (item2.InfoMenuList.Count > 0)
        //                    {
        //                        foreach (var item3 in item2.InfoMenuList)
        //                        {
        //                            XElement xItem3 = new XElement("Menu");
        //                            xItem3.SetAttributeValue("Id", item3.Id);
        //                            xItem3.SetAttributeValue("Guid", item3.Guid);
        //                            xItem3.SetAttributeValue("MenuName", item3.MenuName);
        //                            xItem2.Add(xItem3);
        //                        }
        //                    }

        //                    xItem.Add(xItem2);
        //                }
        //            }

        //            xElement.Add(xItem);
        //        }
        //        return xElement.ToString();
        //    }
        //    return null;
        //}

        //public IList<InfoPlatform> FromItemInfo_Xml()
        //{
        //    IList<InfoPlatform> list = new List<InfoPlatform>();
        //    XElement x = XElement.Parse(_infoItem);
        //    if (x.Name != "InfoItem")
        //        return list;
        //    XAttribute ver = x.Attribute("Version");
        //    if (ver == null || ver.Value != "1")
        //        return list;

        //    //平台
        //    IList<XElement> xitems = x.Descendants("Platform").ToList();
        //    if (xitems.Count > 0)
        //    {
        //        foreach (var xElement in xitems)
        //        {
        //            InfoPlatform item = new InfoPlatform
        //            {
        //                Id = xElement.Attribute("Id").Value.ToInt32Req(),
        //                Guid = xElement.Attribute("Guid").Value,
        //                PlatformName = xElement.Attribute("PlatformName").Value,
        //            };

        //            //系统
        //            IList<XElement> xitems2 = xElement.Descendants("System").ToList();
        //            if (xitems2.Count > 0)
        //            {
        //                foreach (var xElement2 in xitems2)
        //                {
        //                    InfoSystem item2 = new InfoSystem
        //                    {
        //                        Id = xElement2.Attribute("Id").Value.ToInt32Req(),
        //                        Guid = xElement2.Attribute("Guid").Value,
        //                        SystemName = xElement2.Attribute("SystemName").Value,
        //                    };

        //                    //菜单
        //                    IList<XElement> xitems3 = xElement2.Descendants("Menu").ToList();
        //                    if (xitems3.Count > 0)
        //                    {
        //                        foreach (var xElement3 in xitems3)
        //                        {
        //                            InfoMenu item3 = new InfoMenu
        //                            {
        //                                Id = xElement3.Attribute("Id").Value.ToInt32Req(),
        //                                Guid = xElement3.Attribute("Guid").Value,
        //                                MenuName = xElement3.Attribute("MenuName").Value,
        //                            };

        //                            item2.InfoMenuList.Add(item3);
        //                        }
        //                    }

        //                    item.InfoSystemList.Add(item2);
        //                }
        //            }

        //            list.Add(item);
        //        }
        //        return list;
        //    }
        //    return null;
        //}

        #endregion 绑定平台
    }
}