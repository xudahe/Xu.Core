using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Xu.Common
{
    public static class ConvertPath
    {
        //是否是Windows系统
        public static bool _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// 判断操作系统
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReplacePath(this string path)
        {
            //Console.WriteLine($"系统架构：{RuntimeInformation.OSArchitecture}");
            //Console.WriteLine($"系统名称：{RuntimeInformation.OSDescription}");
            //Console.WriteLine($"进程架构：{RuntimeInformation.ProcessArchitecture}");
            //Console.WriteLine($"是否64位操作系统：{Environment.Is64BitOperatingSystem}");

            if (string.IsNullOrEmpty(path))
                return "";
            if (_windows)
                return path.Replace("/", "\\");
            return path.Replace("\\", "/");

        }
    }
}
