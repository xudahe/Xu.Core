using System.Text;

namespace Xu.Common
{
    /// <summary>
    /// Url帮助类
    /// </summary>
    public static class UrlHelper
    {
        /// <summary>
        /// 组合Url地址
        /// </summary>
        /// <param name="url">Url地址，可以是绝对地址、相对地址或虚拟地址</param>
        /// <param name="queryStrings">QueryString键值对，如id=1</param>
        /// <returns></returns>
        public static string Combin(string url, params string[] queryStrings)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(url);
            if (url.Contains("?"))
            {
                foreach (string queryString in queryStrings)
                {
                    if (!string.IsNullOrEmpty(queryString.Trim()))
                        sb.Append("&" + queryString.Trim());
                }
            }
            else
            {
                bool firstQueryString = true;
                foreach (string queryString in queryStrings)
                {
                    if (!string.IsNullOrEmpty(queryString.Trim()))
                    {
                        if (firstQueryString)
                        {
                            sb.Append("?");
                            firstQueryString = false;
                        }
                        else
                            sb.Append("&");
                        sb.Append(queryString);
                    }
                }
            }
            return sb.ToString();
        }
    }
}