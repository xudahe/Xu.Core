using System.Xml.Linq;

namespace Xu.Common
{
    /// <summary>
    /// Xml帮助类
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 解析Xml
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XElement Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            XElement x;
            try { x = XElement.Parse(xml); }
            catch { return null; }

            return x;
        }

        /// <summary>
        /// 解析Xml，校验根元素名
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="rootElementName"></param>
        /// <returns></returns>
        public static XElement Parse(string xml, string rootElementName)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            XElement x;
            try { x = XElement.Parse(xml); }
            catch { return null; }

            if (x.Name != rootElementName)
                return null;

            return x;
        }

        /// <summary>
        /// 解析Xml，校验根元素名和版本
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="rootElementName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static XElement Parse(string xml, string rootElementName, string version)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            XElement x;
            try { x = XElement.Parse(xml); }
            catch { return null; }

            if (x.Name != rootElementName)
                return null;

            XAttribute ver = x.Attribute("Version");
            if (ver == null || ver.Value != version)
                return null;

            return x;
        }

        /// <summary>
        /// 创建Xml
        /// </summary>
        /// <param name="rootElementName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static XElement Create(string rootElementName, string version)
        {
            XElement x = new XElement(rootElementName);
            x.SetAttributeValue("Version", version);
            return x;
        }
    }
}