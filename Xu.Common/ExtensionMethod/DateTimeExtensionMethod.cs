using System;

namespace Xu.Common
{
    /// <summary>
    /// 日期扩展方法类
    /// </summary>
    public static class DateTimeExtensionMethod
    {
        /// <summary>
        /// 转换为指定时间当天的0点0分0秒
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ToDayBegin(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        /// <summary>
        /// 去除指定时间的毫秒
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ToSecondBegin(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
    }
}