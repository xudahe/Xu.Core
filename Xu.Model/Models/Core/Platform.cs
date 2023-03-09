using SqlSugar;
using SqlSugar.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xu.Model.XmlModels;

namespace Xu.Model.Models
{
    /// <summary>
    /// 平台信息表
    /// </summary>
    [SugarTable("Platform", "平台信息表")]
    [TenantAttribute("WMBLOG_MYSQL_1")]
    public class Platform : ModelBase
    {
        /// <summary>
        /// 平台简码
        /// </summary>
        [SugarColumn(ColumnDescription = "平台简码")]
        public string PlatformCode { get; set; }

        /// <summary>
        /// 平台名称
        /// </summary>
        [SugarColumn(ColumnDescription = "平台名称")]
        public string PlatformName { get; set; }

        /// <summary>
        /// 加载序号
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "加载序号")]
        public int Index { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [SugarColumn(ColumnDescription = "主题")]
        public string Theme { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [SugarColumn(ColumnDescription = "true 禁用，false 可用")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue)]
        public string Remark { get; set; }

        /// <summary>
        /// 平台关联系统的id或guid集合，不能同时包含两者
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue, ColumnDescription = "平台关联系统的id或guid集合")]
        public string SystemIds { get; set; }

        #region 绑定系统

        private string _systemInfoItem;

        [SugarColumn(IsIgnore = true)]
        private IList<InfoSystem> _systemInfoList { get; set; }

        /// <summary>
        /// 关联系统List
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue, IsJson = true, ColumnDescription = "关联系统 json")]
        public IList<InfoSystem> SystemInfoList
        {
            get { return _systemInfoList; }
            set
            {
                _systemInfoList = value;
                if (_systemInfoList != null && _systemInfoList.Count > 0)
                    _systemInfoItem = ToItemInfoXml();
                else
                    _systemInfoItem = null;
            }
        }

        /// <summary>
        /// 关联系统Xml
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = int.MaxValue, ColumnDescription = "关联系统 xml")]
        public string SystemInfoXml
        {
            get { return _systemInfoItem; }
            set
            {
                _systemInfoItem = value;
                if (!string.IsNullOrEmpty(_systemInfoItem))
                    _systemInfoList = FromItemInfoXml();
            }
        }

        public string ToItemInfoXml()
        {
            XElement xElement = new XElement("Xml");
            xElement.SetAttributeValue("Version", "1");

            if (_systemInfoList.Count > 0)
            {
                foreach (var item in _systemInfoList)
                {
                    XElement xItem = new XElement("System");
                    xItem.SetAttributeValue("Id", item.Id);
                    xItem.SetAttributeValue("Guid", item.Guid);
                    xItem.SetAttributeValue("SystemName", item.SystemName);
                    xElement.Add(xItem);
                }
                return xElement.ToString();
            }
            return null;
        }

        public IList<InfoSystem> FromItemInfoXml()
        {
            IList<InfoSystem> list = new List<InfoSystem>();
            XElement x = XElement.Parse(_systemInfoItem);
            if (x.Name != "Xml")
                return list;
            XAttribute ver = x.Attribute("Version");
            if (ver == null || ver.Value != "1")
                return list;
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
                    list.Add(item);
                }
                return list;
            }
            return null;
        }

        #endregion 绑定系统
    }
}