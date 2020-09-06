using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xu.Model.Enum;

namespace Xu.Model.Models
{
    /// <summary>
    /// 待办事项
    /// </summary>
    public class Todo : ModelBase
    {
        /// <summary>
        /// 发送人类型
        /// </summary>
        public string SendActorType { get; set; }

        /// <summary>
        /// 发送人Id
        /// </summary>
        public string SendActorId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 紧急程度
        /// </summary>
        public TodoLevel Level { get; set; }

        /// <summary>
        /// 待办类型
        /// </summary>
        public TodoType Type { get; set; }

        /// <summary>
        /// 待办所属功能模块对应的实体
        /// </summary>
        public string RelatedDomain { get; set; }

        /// <summary>
        /// 待办所属功能模块对应的实体Id
        /// </summary>
        public string RelatedDomainId { get; set; }

        /// <summary>
        /// 处理环节
        /// </summary>
        public string ActionType { set; get; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string AdditionalData { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime ReceiveTime { get; set; }

        /// <summary>
        /// 接收人类型
        /// </summary>
        public string ReceiveActorType { get; set; }

        /// <summary>
        /// 接收人Id
        /// </summary>
        public string ReceiveActorId { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? ActionTime { get; set; }

        /// <summary>
        /// 催办次数
        /// </summary>
        public string ReminderNumber { get; set; }

        /// <summary>
        /// 标杆工期（剩余待处理时间）
        /// </summary>
        public int? StandardDate { get; set; }

        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool IsAction { get { return ActionTime.HasValue; } }

        public string ToXml(IDictionary<string, string> data)
        {
            if (data.Keys.Count == 0)
                return string.Empty;

            XElement x = new XElement("ReminderNumber");
            x.SetAttributeValue("Version", "1");

            foreach (var dic in data)
            {
                XElement xData = new XElement("Data");
                xData.SetAttributeValue("Key", dic.Key);
                xData.SetAttributeValue("Value", dic.Value);
                x.Add(xData);
            }
            return x.ToString();
        }

        public IDictionary<string, string> ParseXml()
        {
            IDictionary<string, string> data = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(ReminderNumber))
                return data;

            XElement x;
            try { x = XElement.Parse(ReminderNumber); }
            catch { return data; }

            if (x.Name != "ReminderNumber")
                return data;

            XAttribute ver = x.Attribute("Version");
            if (ver == null || ver.Value != "1")
                return data;
            IList<XElement> xDataList = x.Descendants("Data").ToList();
            foreach (XElement xData in xDataList)
            {
                string key = xData.Attribute("Key").Value;
                string value = xData.Attribute("Value").Value;
                data.Add(key, value);
            }
            return data;
        }
    }
}