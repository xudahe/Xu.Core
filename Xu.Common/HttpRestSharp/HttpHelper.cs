using RestSharp;
using System;
using System.Net;

namespace Xu.Common
{
    /// <summary>
    /// 基于 RestSharp 封装HttpHelper
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Get 请求
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="baseUrl">根域名:http://apk.neters.club/</param>
        /// <param name="url">接口:api/xx/yy</param>
        /// <param name="pragm">参数:id=2&name=老张</param>
        /// <returns></returns>
        public static T GetApi<T>(string baseUrl, string url, string pragm = "")
        {
            var client = new RestSharpClient(baseUrl);

            var request = client.Execute(string.IsNullOrEmpty(pragm)
                ? new RestRequest(url, Method.GET)
                : new RestRequest($"{url}?{pragm}", Method.GET));

            if (request.StatusCode != HttpStatusCode.OK)
            {
                return (T)Convert.ChangeType(request.ErrorMessage, typeof(T));
            }

            dynamic temp = Newtonsoft.Json.JsonConvert.DeserializeObject(request.Content, typeof(T));

            //T result = (T)Convert.ChangeType(request.Content, typeof(T));

            return (T)temp;
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="url">完整的url</param>
        /// <param name="body">post body,可以匿名或者反序列化</param>
        /// <returns></returns>
        public static T PostApi<T>(string url, object body = null)
        {
            var client = new RestClient($"{url}");
            IRestRequest queest = new RestRequest();
            queest.Method = Method.POST;
            queest.AddHeader("Accept", "application/json");
#pragma warning disable CS0618 // '“IRestRequest.RequestFormat”已过时:“Use AddJsonBody or AddXmlBody to tell RestSharp how to serialize the request body”
            queest.RequestFormat = DataFormat.Json;
#pragma warning restore CS0618 // '“IRestRequest.RequestFormat”已过时:“Use AddJsonBody or AddXmlBody to tell RestSharp how to serialize the request body”
#pragma warning disable CS0618 // '“IRestRequest.AddBody(object)”已过时:“Use AddJsonBody or AddXmlBody instead”
            queest.AddBody(body); // 可以使用 JsonSerializer
#pragma warning restore CS0618 // '“IRestRequest.AddBody(object)”已过时:“Use AddJsonBody or AddXmlBody instead”
            var result = client.Execute(queest);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return (T)Convert.ChangeType(result.ErrorMessage, typeof(T));
            }

            dynamic temp = Newtonsoft.Json.JsonConvert.DeserializeObject(result.Content, typeof(T));

            //T result = (T)Convert.ChangeType(request.Content, typeof(T));

            return (T)temp;
        }
    }
}