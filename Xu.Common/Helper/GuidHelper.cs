using System;
using System.Text.RegularExpressions;

namespace Xu.Common
{
    /// <summary>
    /// Guid 公用方法
    /// </summary>
    public class GUIDHelper
    {
        /// <summary>
        /// 生成Guid（32位）
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Guid32(string format = "")
        {
            if (string.IsNullOrWhiteSpace(format))
                return Guid.NewGuid().ToString();
            else
                return Guid.NewGuid().ToString(format);

            // UUID格式：xxxxxxxx - xxxx - xxxx - xxxxxxxxxxxxxxxx(8 - 4 - 4 - 16)
            // GUID格式：xxxxxxxx - xxxx - xxxx - xxxxxx - xxxxxxxxxx(8 - 4 - 4 - 4 - 12)

            // 默认格式：4575c4b3-7997-4f11-acd9-f107258e9adc
            // "N"--32 位：a53a7186b583483aa4580519034e8095
            // "D"--由连字符分隔的 32 位数字：5ae7f002-a989-4345-864b-3bcfbe09e1da
            // "B"--括在大括号中、由连字符分隔的 32 位数字：{d9762660-8461-4c44-b714-8ffad6e1b79c}
            // "P"--括在圆括号中、由连字符分隔的 32 位数字：(694ce704-0a7d-41d5-a25a-4eaedf7db50d)
            // "X"--括在大括号的 4 个十六进制值，其中第 4 个值是 8 个十六进制值的子集（也括在大括号中）：{0x75198f26,0xac4e,0x42c8,{0x96,0x88,0xcc,0x91,0xe0,0xa6,0x9b,0x21}
        }

        /// <summary>
        /// 根据正则表达式判断guid
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        public static bool IsGuidByReg(string strSrc)
        {
            Regex reg = new Regex("^[A-Fa-f0-9]{8}(-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}$", RegexOptions.Compiled);
            return reg.IsMatch(strSrc);
        }
    }
}