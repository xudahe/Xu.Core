using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xu.Common;

namespace Xu.Model
{
    public class DesInsRelaConUnitService
    {
        #region 项目组成员集合

        private string _dpsInfoItem;

        private IList<DspInfoItem> _dpsInfoList { get; set; }

        /// <summary>
        /// 成员集合
        /// </summary>
        public IList<DspInfoItem> DpsInfoList
        {
            get { return _dpsInfoList; }
            set
            {
                _dpsInfoList = value;
                if (_dpsInfoList.Count > 0)
                    _dpsInfoItem = ToItemInfoXml();
                else
                    _dpsInfoItem = null;
            }
        }

        /// <summary>
        /// 成员集合
        /// </summary>
        public string DpsInfoXml
        {
            get { return _dpsInfoItem; }
            set
            {
                _dpsInfoItem = value;
                if (!string.IsNullOrEmpty(_dpsInfoItem))
                    _dpsInfoList = FromItemInfoXml();
            }
        }

        public string ToItemInfoXml()
        {
            XElement xElement = new XElement("Dps");
            xElement.SetAttributeValue("Version", "1");

            if (_dpsInfoList.Count > 0)
            {
                foreach (var item in _dpsInfoList)
                {
                    XElement xItem = new XElement("DpsInfoItem");

                    if (!string.IsNullOrEmpty(item.UserRole))
                        xItem.SetAttributeValue("UserRole", item.UserRole);

                    if (item.UserId.HasValue)
                        xItem.SetAttributeValue("UserId", item.UserId.Value.ToString());
                    else
                        xItem.SetAttributeValue("UserId", "");

                    if (!string.IsNullOrEmpty(item.Department))
                        xItem.SetAttributeValue("Department", item.Department);

                    if (!string.IsNullOrEmpty(item.PhoneNumber))
                        xItem.SetAttributeValue("PhoneNumber", item.PhoneNumber);

                    xElement.Add(xItem);
                }
                return xElement.ToString();
            }
            return null;
        }

        public IList<DspInfoItem> FromItemInfoXml()
        {
            IList<DspInfoItem> list = new List<DspInfoItem>();
            XElement x = XElement.Parse(_dpsInfoItem);
            if (x.Name != "Dps")
                return list;
            XAttribute ver = x.Attribute("Version");
            if (ver == null || ver.Value != "1")
                return list;
            IList<XElement> xitems = x.Descendants("DpsInfoItem").ToList();
            if (xitems.Count > 0)
            {
                foreach (var xElement in xitems)
                {
                    DspInfoItem item = new DspInfoItem();
                    item.UserRole = xElement.Attribute("UserRole").Value;
                    item.UserId = xElement.Attribute("UserId").Value.ToInt32();
                    if (xElement.Value.Contains("Department"))
                        item.Department = xElement.Attribute("Department").Value;
                    if (xElement.Value.Contains("PhoneNumber"))
                        item.PhoneNumber = xElement.Attribute("PhoneNumber").Value;
                    list.Add(item);
                }
                return list;
            }
            return null;
        }

        #endregion 项目组成员集合

        #region 权限

        //private IList<UserPermission> _permissionList;

        ///// <summary>
        ///// 权限存储值，此属性仅供持久化，业务调用使用PermissionList属性
        ///// </summary>
        //public string Permissions { get; set; }

        ///// <summary>
        ///// 权限集合
        ///// </summary>
        //public IList<UserPermission> PermissionList
        //{
        //    get
        //    {
        //        if (_permissionList == null)
        //        {
        //            if (string.IsNullOrEmpty(Permissions))
        //                _permissionList = new List<UserPermission>();
        //            else
        //                _permissionList = Permissions.SplitString(",").Select(s => s.ToEnumReq<UserPermission>()).ToList();
        //        }

        //        return _permissionList;
        //    }
        //    set
        //    {
        //        _permissionList = value;
        //        Permissions = value.Select(s => s.GetValue().ToString()).JoinToString(",", true);
        //    }
        //}

        #endregion 权限
    }

    /// <summary>
    /// 临时实体
    /// </summary>
    public class DspInfoItem
    {
        /// <summary>
        /// 角色
        /// </summary>
        public string UserRole { set; get; }

        /// <summary>
        /// 项目组成员
        /// </summary>
        public int? UserId { set; get; }

        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}