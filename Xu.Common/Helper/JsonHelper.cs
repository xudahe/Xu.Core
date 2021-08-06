using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Xu.Common
{
    public class JsonHelper
    {
        /// <summary>
        /// 转换List<T>的数据为JSON格式
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="vals">列表值</param>
        /// <returns>JSON格式数据</returns>
        public static string JSON<T>(List<T> vals)
        {
            StringBuilder st = new StringBuilder();

            DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));

            foreach (T city in vals)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    s.WriteObject(ms, city);
                    st.Append(Encoding.UTF8.GetString(ms.ToArray()));
                }

                //分隔符
                int index = vals.IndexOf(city);
                if (vals.Count - 1 != index)
                {
                    st.Append(",");
                }
            }

            //首尾插入
            if (vals.Count > 0)
            {
                st.Insert(0, "[");
                st.Append("]");
            }

            return st.ToString();
        }

        /// <summary>
        /// 转换对象为JSON格式数据(序列化)
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>字符格式的JSON数据</returns>
        public static string GetJSON<T>(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// JSON格式字符转换为T类型的对象(反序列化)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T ParseFormByJson<T>(string jsonStr)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
            {
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}