using System.Xml.Linq;

namespace Xu.Common
{
    /// <summary>
    /// Xml操作的扩展方法
    /// </summary>
    public static class XmlLinqExtensionMethod
    {
        /// <summary>
        /// 取XElement值，如果为空返回空字符串
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetValue(this XElement e)
        {
            if (e == null)
                return string.Empty;

            return e.Value;
        }

        /// <summary>
        /// 取XAttribute值，如果为空返回空字符串
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string GetValue(this XAttribute a)
        {
            if (a == null)
                return string.Empty;

            return a.Value;
        }

        /// <summary>
        /// 取XElement子元素值
        /// </summary>
        /// <param name="e"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string GetElementValue(this XElement e, string elementName)
        {
            if (e == null)
                return string.Empty;

            //XElement element = e.Descendants(elementName).FirstOrDefault().GetValue();
            XElement element = e.Element(elementName);
            if (element == null)
                return string.Empty;

            return element.Value;
        }

        /// <summary>
        /// 取XElement的Attribute属性值
        /// </summary>
        /// <param name="e"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string GetAttributeValue(this XElement e, string attributeName)
        {
            if (e == null)
                return string.Empty;

            XAttribute attr = e.Attribute(attributeName);
            if (attr == null)
                return string.Empty;

            return attr.Value;
        }

        /// <summary>
        /// 添加Xml元素
        /// </summary>
        /// <param name="e"></param>
        /// <param name="elementName"></param>
        /// <param name="elementValue"></param>
        public static void AddElement(this XElement e, string elementName, string elementValue)
        {
            e.Add(new XElement(elementName, elementValue));
        }
    }
}