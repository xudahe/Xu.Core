using System.Collections.Generic;

namespace Xu.Common
{
    /// <summary>
    /// Double字典运算，最常用的求和和求百分比
    /// </summary>
    public static class DoubleDictionaryMathExtensionMethod
    {
        /// <summary>
        /// Float字典相加
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static IDictionary<string, double> Add(this IDictionary<string, double> dict1, IDictionary<string, double> dict2)
        {
            IDictionary<string, double> dict = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> pair in dict1)
            {
                if (dict.ContainsKey(pair.Key))
                    dict[pair.Key] += dict1[pair.Key];
                else
                    dict.Add(pair.Key, dict1[pair.Key]);
            }
            foreach (KeyValuePair<string, double> pair in dict2)
            {
                if (dict.ContainsKey(pair.Key))
                    dict[pair.Key] += dict2[pair.Key];
                else
                    dict.Add(pair.Key, dict2[pair.Key]);
            }
            return dict;
        }

        /// <summary>
        /// Double字典相除
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static IDictionary<string, double> Div(this IDictionary<string, double> dict1, IDictionary<string, double> dict2)
        {
            IDictionary<string, double> dict = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> pair in dict1)
            {
                if (dict2.ContainsKey(pair.Key))
                    if (dict2[pair.Key] != 0)
                        dict.Add(pair.Key, pair.Value / dict2[pair.Key]);
            }
            foreach (KeyValuePair<string, double> pair in dict2)
            {
                if (!dict1.ContainsKey(pair.Key))
                    dict.Add(pair.Key, 0);
            }
            return dict;
        }
    }
}