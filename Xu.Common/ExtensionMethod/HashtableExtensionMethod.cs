using System.Collections;

namespace Xu.Common
{
    /// <summary>
    /// Hashtable扩展方法
    /// </summary>
    public static class HashtableExtensionMethod
    {
        /// <summary>
        /// 添加项，仅在Hashtable中没有键时
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddWhenNotExisted(this Hashtable ht, object key, object value)
        {
            if (!ht.ContainsKey(key))
                ht.Add(key, value);
        }

        /// <summary>
        /// 添加项，当Hashtable中没有同名项时直接添加，已有同名项时则更新值
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate(this Hashtable ht, object key, object value)
        {
            if (ht.ContainsKey(key))
                ht[key] = value;
            else
                ht.Add(key, value);
        }
    }
}