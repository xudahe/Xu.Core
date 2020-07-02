using System;
using System.Collections;
using System.Collections.Generic;

namespace Xu.Common
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

        /*
		/// <summary>
		/// 取枚举名称值对
		/// </summary>
		/// <param name="avail">是否只取有效枚举（根据EnumDisabledAttribute），null表示取所有，true表示只取有效（EnumDisabledAttribute=false）的枚举，false表示只取无效（EnumDisabledAttribute=true）的枚举</param>
		/// <param name="customSort">是否自定义排序（根据EnumIndexAttribute），true表示根据EnumIndexAttribute值排序，false表示按默认排序（枚举值）</param>
		/// <returns></returns>
		public static IDictionary<string, object> GetNameValue(bool? avail = null, bool customSort = false)
		{
			Type enumType = typeof(T);

			if (!enumType.IsEnum)
				throw new NotSupportedException(string.Format("{0}必须为枚举类型。", enumType));

			IList<Entity<string, object, bool, string>> list = new List<Entity<string, object, bool, string>>();
			foreach (var e in Enum.GetValues(enumType))
			{
				string name = e.ToString();

				bool disabled = false;
				if (avail.HasValue)
				{
					var disabledAttributes = (EnumDisabledAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumDisabledAttribute), false);
					if (disabledAttributes.Count() > 0)
						disabled = disabledAttributes.First().Value;
				}

				object value = enumType.GetField(name).GetRawConstantValue();

				string index = value.ToString();
				if (customSort)
				{
					var indexAttributes = (EnumIndexAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumIndexAttribute), false);
					if (indexAttributes.Count() > 0)
						index = indexAttributes.First().Value;
				}

				list.Add(new Entity<string, object, bool, string>(name, value, disabled, index));
			}

			if (customSort)
				list = list.OrderBy(s => s.Property4).ToList();

			IDictionary<string, object> dict = new Dictionary<string, object>();
			foreach (var entity in list)
			{
				if (!avail.HasValue || avail.Value != entity.Property3)
					dict.Add(entity.Property1, entity.Property2);
			}

			return dict;
		}

		/// <summary>
		/// 取枚举名称
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static string GetName(string t)
		{
			T? e = t.ToEnum<T>();

			if (e.HasValue)
				return e.Value.ToString();

			return string.Empty;
		}

		/// <summary>
		/// 取枚举名称
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static string GetText(string t)
		{
			T? e = t.ToEnum<T>();

			if (e.HasValue)
			{
				string name = e.Value.ToString();
				var textAttributes = (EnumTextAttribute[])typeof(T).GetField(name).GetCustomAttributes(typeof(EnumTextAttribute), false);
				if (textAttributes.Count() > 0)
					return textAttributes.First().Value;

				return name;
			}

			return string.Empty;
		}

		/// <summary>
		/// 解析字符串为Enum，与Enum.Parse不同，不在枚举类定义的值无法解析
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static T? Parse(string str)
		{
			return str.ToEnum<T>();
		}

		/// <summary>
		/// 解析字符串为Enum，与Enum.Parse不同，不在枚举类定义的值无法解析，如无法解析抛出异常
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static T ParseReq(string str)
		{
			return str.ToEnumReq<T>();
		}

		/// <summary>
		/// 解析字符串为Enum，与Enum.Parse不同，不在枚举类定义的值无法解析
		/// </summary>
		/// <param name="str"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryParse(string str, out T value)
		{
			T? t = str.ToEnum<T>();
			if (t.HasValue)
			{
				value = t.Value;
				return true;
			}
			else
			{
				value = default(T);
				return false;
			}
		}

		/// <summary>
		/// 解析字符串为Enum，与Enum.Parse不同，不在枚举类定义的值无法解析
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static T ParseOrDefault(string str)
		{
			T? t = str.ToEnum<T>();
			if (t.HasValue)
				return t.Value;
			else
				return default(T);
		}

		/// <summary>
		/// 解析字符串为Enum，与Enum.Parse不同，不在枚举类定义的值无法解析
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T ParseOrDefault(string str, T defaultValue)
		{
			T? t = str.ToEnum<T>();
			if (t.HasValue)
				return t.Value;
			else
				return def gaultValue;
		}
		*/
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