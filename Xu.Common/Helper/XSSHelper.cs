using System.Text.RegularExpressions;

namespace Xu.Common
{
    /// <summary>
    /// Xss处理的帮助类
    /// </summary>
    /// <remarks>XSS跨站脚本攻击是在表单之类的输入框输入js代码</remarks>
    public static class XSSHelper
    {
        /// <summary>
        /// XSS过滤
        /// </summary>
        /// <param name="html">html代码</param>
        /// <returns>过滤结果</returns>
        public static string XssFilter(string html)
        {
            string str = HtmlFilter(html);
            return str;
        }

        /// <summary>
        /// 过滤HTML标记
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string HtmlFilter(string Htmlstring)
        {
            // 写自己的处理逻辑即可，下面是把 匹配到<[^>]*>全部过滤掉，建议慎用，只是一个例子
            string result = Regex.Replace(Htmlstring, @"<[^>]*>", string.Empty);
            return result;
        }
    }
}

//反射型XSS：顾名思义在于“反射”这个一来一回的过程。反射型XSS的触发有后端的参与，而之所以触发XSS是因为后端解析用户在前端输入的带有XSS性质的脚本或者脚本的data URI编码，后端解析用户输入处理后返回给前端，由浏览器解析这段XSS脚本，触发XSS漏洞。因此如果要避免反射性XSS，则必须需要后端的协调，在后端解析前端的数据时首先做相关的字串检测和转义处理；同时前端同样也许针对用户的数据做excape转义，保证数据源的可靠性。

//存储型XSS：则是直接将xss语句插入到网站的正常页面中（通常都是留言板），然后用户只要访问了这些页面，就会自动执行其中的xss语句。

//反射型XSS：主要做法是将 Javascript 代码加入 URL 地址的请求参数里，若 Web 应用程序存在漏洞，请求参数会在页面直接输出，用户点击类似的恶意链接就可能遭受攻击。