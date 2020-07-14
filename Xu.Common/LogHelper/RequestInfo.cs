using System.Collections.Generic;

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

    public class RequestInfo
    {
        public string Ip { get; set; }
        public string Url { get; set; }
        public string Datetime { get; set; }
        public string Date { get; set; }
        public string Week { get; set; }
    }
}