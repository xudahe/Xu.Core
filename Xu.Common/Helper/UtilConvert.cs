using System;
using System.Collections.Generic;

namespace Xu.Common
{
    /// <summary>
    ///
    /// </summary>
    public static class UtilConvert
    {
        public static object ChangeType(this object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }

            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }

            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }

        public static object ChangeTypeList(this object value, Type type)
        {
            if (value == null) return default;

            var gt = typeof(List<>).MakeGenericType(type);
            dynamic lis = Activator.CreateInstance(gt);

            var addMethod = gt.GetMethod("Add");
            string values = value.ToString();
            if (values != null && values.StartsWith("(") && values.EndsWith(")"))
            {
                string[] splits;
                if (values.Contains("\",\""))
                {
                    splits = values.Remove(values.Length - 2, 2)
                        .Remove(0, 2)
                        .Split("\",\"");
                }
                else
                {
                    splits = values.Remove(0, 1)
                        .Remove(values.Length - 2, 1)
                        .Split(",");
                }

                foreach (var split in splits)
                {
                    var str = split;
                    if (split.StartsWith("\"") && split.EndsWith("\""))
                    {
                        str = split.Remove(0, 1)
                            .Remove(split.Length - 2, 1);
                    }

                    addMethod.Invoke(lis, new object[] { ChangeType(str, type) });
                }
            }

            return lis;
        }
    }
}