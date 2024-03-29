﻿using System;
using System.Reflection;

namespace Xu.EnumHelper
{
    /// <summary>
    /// 枚举扩展函数
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 获得枚举字段的特性(Attribute)，该属性不允许多次定义。
        /// </summary>
        public static string GetAttributeValue(this Enum thisValue)
        {
            FieldInfo field = thisValue.GetType().GetField(thisValue.ToString());
            var attr = (Attribute.GetCustomAttribute(field, typeof(Desc)) as Desc);
            if (attr == null) return string.Empty;
            return (Attribute.GetCustomAttribute(field, typeof(Desc)) as Desc).Value;
        }

        /// <summary>
        /// 获得枚举字段的特性(Attribute)，该属性不允许多次定义。
        /// </summary>
        public static T GetAttribute<T>(this Enum thisValue) where T : class
        {
            FieldInfo field = thisValue.GetType().GetField(thisValue.ToString());
            var attr = (Attribute.GetCustomAttribute(field, typeof(T)) as T);
            return attr;
        }

        /// <summary>
        /// 获得枚举字段的名称。
        /// </summary>
        /// <returns></returns>
        public static string GetName(this Enum thisValue)
        {
            return Enum.GetName(thisValue.GetType(), thisValue);
        }

        /// <summary>
        /// 获得枚举字段的值。
        /// </summary>
        /// <returns></returns>
        public static T GetValue<T>(this Enum thisValue)
        {
            return (T)Enum.Parse(thisValue.GetType(), thisValue.ToString());
        }
    }

    /// <summary>
    /// 字段或属性的中文解释属性
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Desc : Attribute
    {
        /// <summary>
        /// 获得字段或属性的中文解释.
        /// </summary>
        /// <value>字段或属性的中文解释.</value>
        public string Value { get; private set; }

        /// <summary>
        /// 初始化创建一个 <see cref="Desc"/> 类的实例, 用于指示字段或属性的解释说明.
        /// </summary>
        /// <param name="value">字段或属性的解释说明.</param>
        public Desc(string value)
        {
            Value = value;
        }
    }

    #region 用法

    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    var value = MyType.one.GetValue<int>();//值为1
    //    var name = MyType.one.GetName();//值为one
    //    var desc = MyType.one.GetAttributeValue();//值为一
    //}

    //public enum MyType
    //{
    //    [Desc("一")]
    //    one = 1,

    //    [Desc("二")]
    //    two = 2
    //}

    #endregion 用法
}