using System;

namespace Xu.Common
{
    public class LogInfo
    {
        public DateTime Datetime { get; set; }
        public string Content { get; set; }
        public string ClientIP { get; set; }
        public string LogColor { get; set; }
        public int Import { get; set; } = 0;
    }

    public class RequestInfo
    {
        /// <summary>
        /// 客户端ip
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }

        public string Datetime { get; set; }
        public string Week { get; set; }
    }

    public class UserAccessModel
    {
        /// <summary>
        /// 操作人
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 客户端ip
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 服务端ip
        /// </summary>
        public string ServiceIP { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public string BeginTime { get; set; }

        /// <summary>
        /// 时长(毫秒)
        /// </summary>
        public string OPTime { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string RequestMethod { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestData { get; set; }

        /// <summary>
        /// 浏览器类型
        /// </summary>
        public string Agent { get; set; }
    }
}