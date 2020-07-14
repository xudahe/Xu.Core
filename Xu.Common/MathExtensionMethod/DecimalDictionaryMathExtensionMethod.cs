using System.Collections.Generic;

namespace Xu.Common
{
    /// <summary>
    /// Decimal字典运算，最常用的求和和求百分比
    /// </summary>
    public static class DecimalDictionaryMathExtensionMethod
    {
        /// <summary>
        /// Decimal字典相加，一个字典里有，一个字典里没有的项，没有的按0计算
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static IDictionary<string, decimal> Add(this IDictionary<string, decimal> dict1, IDictionary<string, decimal> dict2)
        {
            IDictionary<string, decimal> dict = new Dictionary<string, decimal>();
            foreach (KeyValuePair<string, decimal> pair in dict1)
            {
                if (dict.ContainsKey(pair.Key))
                    dict[pair.Key] += dict1[pair.Key];
                else
                    dict.Add(pair.Key, dict1[pair.Key]);
            }
            foreach (KeyValuePair<string, decimal> pair in dict2)
            {
                if (dict.ContainsKey(pair.Key))
                    dict[pair.Key] += dict2[pair.Key];
                else
                    dict.Add(pair.Key, dict2[pair.Key]);
            }
            return dict;
        }

        /// <summary>
        /// Decimal字典相除，如果某个键只在被除数字典存在，不生成此项计算结果
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static IDictionary<string, decimal> Div(this IDictionary<string, decimal> dict1, IDictionary<string, decimal> dict2)
        {
            IDictionary<string, decimal> dict = new Dictionary<string, decimal>();
            foreach (KeyValuePair<string, decimal> pair in dict1)
            {
                if (dict2.ContainsKey(pair.Key))
                    if (dict2[pair.Key] != 0)
                        dict.Add(pair.Key, pair.Value / dict2[pair.Key]);
            }
            foreach (KeyValuePair<string, decimal> pair in dict2)
            {
                if (!dict1.ContainsKey(pair.Key))
                    dict.Add(pair.Key, 0);
            }
            return dict;
        }
    }
}