using System.Collections.Generic;

namespace Xu.Common
{
    /// <summary>
    /// Float字典运算，最常用的求和和求百分比
    /// </summary>
    public static class FloatDictionaryMathExtensionMethod
    {
        /// <summary>
        /// Float字典相加
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static IDictionary<string, float> Add(this IDictionary<string, float> dict1, IDictionary<string, float> dict2)
        {
            IDictionary<string, float> dict = new Dictionary<string, float>();
            foreach (KeyValuePair<string, float> pair in dict1)
            {
                if (dict.ContainsKey(pair.Key))
                    dict[pair.Key] += dict1[pair.Key];
                else
                    dict.Add(pair.Key, dict1[pair.Key]);
            }
            foreach (KeyValuePair<string, float> pair in dict2)
            {
                if (dict.ContainsKey(pair.Key))
                    dict[pair.Key] += dict2[pair.Key];
                else
                    dict.Add(pair.Key, dict2[pair.Key]);
            }
            return dict;
        }

        /// <summary>
        /// Float字典相除
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static IDictionary<string, float> Div(this IDictionary<string, float> dict1, IDictionary<string, float> dict2)
        {
            IDictionary<string, float> dict = new Dictionary<string, float>();
            foreach (KeyValuePair<string, float> pair in dict1)
            {
                if (dict2.ContainsKey(pair.Key))
                    if (dict2[pair.Key] != 0)
                        dict.Add(pair.Key, pair.Value / dict2[pair.Key]);
            }
            foreach (KeyValuePair<string, float> pair in dict2)
            {
                if (!dict1.ContainsKey(pair.Key))
                    dict.Add(pair.Key, 0);
            }
            return dict;
        }
    }
}