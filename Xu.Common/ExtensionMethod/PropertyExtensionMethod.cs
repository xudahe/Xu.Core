using System.Reflection;

namespace Xu.Common
{
    /// <summary>
    /// 对象扩展方法
    /// </summary>
    public static class PropertyExtensionMethod
    {
        /// <summary>
        /// 取指定属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(this T t, string name)
        {
            if (t == null)
                return null;

            PropertyInfo property = t.GetType().GetProperty(name);
            if (property == null)
                return null;

            return property.GetValue(t, null);
        }

        /// <summary>
        /// 取Name属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object GetName<T>(this T t)
        {
            return t.GetPropertyValue("Name");
        }

        /// <summary>
        /// 取Value属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object GetValue<T>(this T t)
        {
            return t.GetPropertyValue("Value");
        }
    }
}