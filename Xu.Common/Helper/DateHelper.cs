using System;
using System.Collections.Generic;

namespace Xu.Common
{
    /// <summary>
    /// 日期时间相关的帮助类
    /// </summary>
    public static class DateHelper
    {
        public static DateTime StampToDateTime(string time)
        {
            time = time.Substring(0, 10);
            double timestamp = Convert.ToInt64(time);
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
            return dateTime;
        }

        public static string TimeSubTract(DateTime time1, DateTime time2)
        {
            TimeSpan subTract = time1.Subtract(time2);
            return $"{subTract.Days} 天 {subTract.Hours} 时 {subTract.Minutes} 分 ";
        }

        /// <summary>
        /// 生成int数组
        /// </summary>
        /// <param name="startWith">开始于</param>
        /// <param name="count">数组数量</param>
        /// <param name="incrementBy">增量</param>
        /// <returns></returns>
        public static int[] BuildIntArray(int startWith, int count, int incrementBy = 1)
        {
            int[] array = new int[count];
            for (int i = 0; i < count; i++)
                array[i] = startWith + i * incrementBy;

            return array;
        }

        /// <summary>
        /// 根据月份计算所属季度，季度取值范围在[1-4]
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public static int GetQuarter(int month)
        {
            if (month >= 1 && month <= 3)
                return 1;
            if (month >= 4 && month <= 6)
                return 2;
            if (month >= 7 && month <= 9)
                return 3;
            if (month >= 10 && month <= 12)
                return 4;
            if (month < 1)
                return 1;
            return 4;
        }

        /// <summary>
        /// 取季度数组
        /// </summary>
        /// <returns></returns>
        public static int[] GetQuarters()
        {
            return new[] { 1, 2, 3, 4 };
        }

        /// <summary>
        /// 根据季度取包含月份
        /// </summary>
        /// <param name="quarter"></param>
        /// <returns></returns>
        public static int[] GetMonths(int quarter)
        {
            if (quarter == 1)
                return new[] { 1, 2, 3 };
            if (quarter == 2)
                return new[] { 4, 5, 6 };
            if (quarter == 3)
                return new[] { 7, 8, 9 };
            if (quarter == 4)
                return new[] { 10, 11, 12 };
            return new int[] { };
        }

        /// <summary>
        /// 取月份数组
        /// </summary>
        /// <returns></returns>
        public static int[] GetMonths()
        {
            return new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        }

        /// <summary>
        /// 生成从指定年份到当前年份的数组
        /// </summary>
        /// <param name="beginYear"></param>
        /// <returns></returns>
        public static int[] GetYears(int beginYear)
        {
            return GetYears(beginYear, DateTime.Now.Year);
        }

        /// <summary>
        /// 生成从两个年份区间的数组
        /// </summary>
        /// <param name="beginYear"></param>
        /// <param name="endYear"></param>
        /// <returns></returns>
        public static int[] GetYears(int beginYear, int endYear)
        {
            if (beginYear > endYear)
            {
                int temp = beginYear;
                beginYear = endYear;
                endYear = temp;
            }

            List<int> list = new List<int>();
            for (int i = beginYear; i <= endYear; i++)
                list.Add(i);
            return list.ToArray();
        }

        /// <summary>
        /// //获取当前周几
        /// </summary>
        /// <returns></returns>
        public static string GetWeek()
        {
            string week = DateTime.Now.DayOfWeek switch
            {
                DayOfWeek.Monday => "周一",
                DayOfWeek.Tuesday => "周二",
                DayOfWeek.Wednesday => "周三",
                DayOfWeek.Thursday => "周四",
                DayOfWeek.Friday => "周五",
                DayOfWeek.Saturday => "周六",
                DayOfWeek.Sunday => "周日",
                _ => "N/A",
            };
            return week;
        }

        /// <summary>
        /// 获取当前时间的时间戳
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static string DateToTimeStamp(this DateTime thisValue)
        {
            TimeSpan ts = thisValue - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}