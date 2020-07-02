using System.Collections;
using System.Collections.Generic;

namespace Xu.Common
{
    /// <summary>
    /// 泛型字典扩展方法
    /// </summary>
    public static class GenericDictionaryExtensionMethod
    {
        /// <summary>
        /// 从字典中取值，如没有可指定默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static V GetValue<T, V>(this IDictionary<T, V> dict, T key, V defaultValue)
        {
            if (dict != null && dict.ContainsKey(key))
                return dict[key];

            return defaultValue;
        }

        //不需要GetValueReq方法，直接Dictionary[key]获取即可获得相同效果

        /// <summary>
        /// 从字典中取值，如没有返回可空值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static V? GetValueOrNullable<T, V>(this IDictionary<T, V?> dict, T key) where V : struct
        {
            if (dict != null && dict.ContainsKey(key))
                return dict[key];
            return null;
        }

        /// <summary>
        /// 从字典中取值，如没有返回可空值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static V? GetValueOrNullable<T, V>(this IDictionary<T, V> dict, T key) where V : struct
        {
            if (dict != null && dict.ContainsKey(key))
                return dict[key];
            return null;
        }

        /// <summary>
        /// 从字典中取值，如没有返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static V GetValueOrNull<T, V>(this IDictionary<T, V> dict, T key) where V : class
        {
            if (dict != null && dict.ContainsKey(key))
                return dict[key];
            return null;
        }

        /// <summary>
        /// 当字典中没有同名项时添加项
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddWhenNotExisted<T, V>(this IDictionary<T, V> dict, T key, V value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
        }

        /// <summary>
        /// 添加项，当字典中没有同名项时直接添加，已有同名项时更新值
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<T, V>(this IDictionary<T, V> dict, T key, V value)
        {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }
    }

    /// <summary>
    /// 字典扩展方法类
    /// </summary>
    public static class DictionaryExtensionMethod
    {
        /// <summary>
        /// 从字典中取值
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object GetValue(this IDictionary dict, object key, object defaultValue)
        {
            if (dict != null && dict.Contains(key))
                return dict[key];

            return defaultValue;
        }

        //不需要GetValueReq方法，直接Dictionary[key]获取即可获得相同效果

        /// <summary>
        /// 从字典中取值
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValueOrNull(this IDictionary dict, object key)
        {
            if (dict != null && dict.Contains(key))
                return dict[key];
            return null;
        }

        /// <summary>
        /// 添加项，仅在IDictionary中没有同名项时
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddWhenNotExisted(this IDictionary dict, object key, object value)
        {
            if (!dict.Contains(key))
                dict.Add(key, value);
        }

        /// <summary>
        /// 添加项，当IDictionary中没有同名项时直接添加，已有同名项时则更新值
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate(this IDictionary dict, object key, object value)
        {
            if (dict.Contains(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }

        /// <summary>
        /// 复制字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static IDictionary<T, V> Clone<T, V>(this IDictionary<T, V> dict)
        {
            IDictionary<T, V> newDict = new Dictionary<T, V>();
            if (dict != null)
            {
                foreach (KeyValuePair<T, V> pair in dict)
                    newDict.Add(pair.Key, pair.Value);
            }

            return newDict;
        }
    }
}