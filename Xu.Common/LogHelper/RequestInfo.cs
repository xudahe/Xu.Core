using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xu.Common
{
    public class ApiWeek
    {
        public string Week { get; set; }
        public string Url { get; set; }
        public int Count { get; set; }
    }

    public class ApiDate
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }

    public class ActiveUserVM
    {
        public string User { get; set; }
        public int Count { get; set; }
    }

    public class RequestApiWeekView
    {
        public List<string> Columns { get; set; }
        public string Rows { get; set; }
    }

    public class AccessApiDateView
    {
        public string[] Columns { get; set; }
        public List<ApiDate> Rows { get; set; }
    }

    public class ApiLogAopInfo
    {
        /// <summary>
        /// 请求时间
        /// </summary>
        public string RequestTime { get; set; } = string.Empty;

        /// <summary>
        /// 操作人员
        /// </summary>
        public string OpUserName { get; set; } = string.Empty;

        /// <summary>
        /// 请求方法名
        /// </summary>
        public string RequestMethodName { get; set; } = string.Empty;

        /// <summary>
        /// 请求参数名
        /// </summary>
        public string RequestParamsName { get; set; } = string.Empty;

        /// <summary>
        /// 请求参数数据JSON
        /// </summary>
        public string RequestParamsData { get; set; } = string.Empty;

        /// <summary>
        /// 请求响应间隔时间
        /// </summary>
        public string ResponseIntervalTime { get; set; } = string.Empty;

        /// <summary>
        /// 响应时间
        /// </summary>
        public string ResponseTime { get; set; } = string.Empty;

        /// <summary>
        /// 响应结果
        /// </summary>
        public string ResponseJsonData { get; set; } = string.Empty;
    }

    public class ApiLogAopExInfo
    {
        public ApiLogAopInfo ApiLogAopInfo { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public string InnerException { get; set; } = string.Empty;

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ExMessage { get; set; } = string.Empty;
    }
}