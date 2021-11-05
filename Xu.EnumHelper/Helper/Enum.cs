using System;
using System.Collections;
using System.Collections.Generic;

namespace Xu.EnumHelper
{
    /// <summary>
    /// 泛型枚举静态帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Enum<T> where T : struct
    {
        /// <summary>
        /// 将枚举转换成IEnumerable&lt;aT&gt;
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static IEnumerable<T> AsEnumerable()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
                throw new NotSupportedException(string.Format("{0}必须为枚举类型。", enumType));

            EnumQuery<T> query = new EnumQuery<T>();
            return query;
        }
    }

    internal class EnumQuery<T> : IEnumerable<T>
    {
        private List<T> _list;

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            Array values = Enum.GetValues(typeof(T));
            _list = new List<T>(values.Length);
            foreach (var value in values)
                _list.Add((T)value);

            return _list.GetEnumerator();
        }

        #endregion IEnumerable<T> Members

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}