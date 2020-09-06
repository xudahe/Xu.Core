using System;

namespace Xu.Common
{
    /// <summary>
    /// Guid 各种模式生成方法
    /// </summary>
    public class GUIDHelper
    {
        public static string GetGuid(string format = "")
        {
            if (string.IsNullOrWhiteSpace(format))
                return Guid.NewGuid().ToString();
            else
                return Guid.NewGuid().ToString(format);

            // UUID格式：xxxxxxxx - xxxx - xxxx - xxxxxxxxxxxxxxxx(8 - 4 - 4 - 16)
            // GUID格式：xxxxxxxx - xxxx - xxxx - xxxxxx - xxxxxxxxxx(8 - 4 - 4 - 4 - 12)

            // "N"--32 位：
            // "D"--由连字符分隔的 32 位数字：
            // "B"--括在大括号中、由连字符分隔的 32 位数字：
            // "P"--括在圆括号中、由连字符分隔的 32 位数字：
            // "X"--括在大括号的 4 个十六进制值，其中第 4 个值是 8 个十六进制值的子集（也括在大括号中）：
        }
    }
}