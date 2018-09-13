using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QK.Framework.Core.Network
{
    /// <summary>
    /// Http请求帮助类
    /// </summary>
    public class HttpHelper
    {
        public const string ContentType = "Content-Type";
        public static HttpClientHandler httpClientHandler = new HttpClientHandler { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip };

        public static async Task<HttpResponseMessage> GetResponseAsync(string url, Dictionary<string, string> parms = null, Dictionary<string, string> headers = null, string cookies = "", int timeout = 10)
        {
            var hasContentType = false;
            var result = new HttpResult();
            if (string.IsNullOrWhiteSpace(url))
                throw new Exception("Url不能为空");

            var httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = new TimeSpan(new DateTime().AddSeconds(timeout).Ticks);

            var parmStr = "";
            if (parms != null && parms.Count > 0)
            {
                foreach (var m in parms.Keys)
                {
                    parmStr += $"{m}={parms[m]}&";
                }
                parmStr = parmStr.TrimEnd('&');
                url += "?" + parmStr;
            }
            if (headers != null && headers.Count > 0)
            {
                var requestHeaders = httpClient.DefaultRequestHeaders;
                foreach (var m in headers.Keys)
                {
                    if (m == ContentType)
                    {
                        hasContentType = true;
                        continue;
                    }
                    requestHeaders.Add(m, headers[m]);
                }
            }

            HttpResponseMessage response = null;

            if (hasContentType || !string.IsNullOrWhiteSpace(cookies))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                if (hasContentType)
                {
                    request.Headers.Add(ContentType, headers[ContentType]);
                }
                if (!string.IsNullOrWhiteSpace(cookies))
                {
                    request.Headers.Add("Cookie", cookies);
                }
                response = await httpClient.SendAsync(request);
            }
            else
            {
                response = await httpClient.GetAsync(url);
            }

            return response;
        }

        /// <summary>
        /// 发送一个get请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="parms">参数字典</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">Cookie字符串</param>
        /// <param name="timeout">超时时间默认5秒(单位 秒)</param>
        /// <returns></returns>
        public static async Task<HttpResult> GetStringAsync(string url, Dictionary<string, string> parms = null, Dictionary<string, string> headers = null, string cookies = "", int timeout = 5)
        {
            var result = new HttpResult();
            var response = await GetResponseAsync(url, parms, headers, cookies, timeout);
            result.success = true;
            result.code = response.StatusCode;
            result.content = await response.Content.ReadAsStringAsync();

            return result;
        }

        /// <summary>
        /// 发送一个Post请求返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parms"></param>
        /// <param name="headers"></param>
        /// <param name="cookies"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<HttpResult> PostFormUrlEncodedContentAsync(string url, Dictionary<string, string> parms = null, Dictionary<string, string> headers = null, string cookies = "", int timeout = 10)
        {
            var result = new HttpResult();
            try
            {
                var reponse = await PostFormUrlAsync(url, parms, headers, cookies, timeout);
                result.success = true;
                result.code = reponse.StatusCode;
                result.content = await reponse.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.content = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// HttpResponseMessage
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parms"></param>
        /// <param name="headers"></param>
        /// <param name="cookies"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostFormUrlAsync(string url, Dictionary<string, string> parms = null, Dictionary<string, string> headers = null, string cookies = "", int timeout = 10)
        {
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = new TimeSpan(new DateTime().AddSeconds(timeout).Ticks);
            if (headers != null && headers.Count > 0)
            {
                var requestHeaders = httpClient.DefaultRequestHeaders;
                foreach (var m in headers.Keys)
                {
                    if (m == ContentType)
                    {
                        continue;
                    }
                    requestHeaders.Add(m, headers[m]);
                }
            }
            var formContent = new FormUrlEncodedContent(parms);
            if (!string.IsNullOrWhiteSpace(cookies))
            {
                formContent.Headers.Add("Cookie", cookies);
            }
            return await httpClient.PostAsync(url, formContent);
        }

        public static async Task<HttpResult> PostFormBodyEncodedContentAsync(string url, object data, Dictionary<string, string> headers = null, string cookies = "", int timeout = 10)
        {
            var result = new HttpResult();
            try
            {
                var reponse = await PostFormBodyAsync(url, data, headers, cookies, timeout);
                result.success = true;
                result.code = reponse.StatusCode;
                result.content = await reponse.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.content = ex.Message;
            }

            return result;
        }

        public static async Task<HttpResponseMessage> PostFormBodyAsync(string url, object data, Dictionary<string, string> headers = null, string cookies = "", int timeout = 10)
        {
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = new TimeSpan(new DateTime().AddSeconds(timeout).Ticks);
            if (headers != null && headers.Count > 0)
            {
                var requestHeaders = httpClient.DefaultRequestHeaders;
                foreach (var m in headers.Keys)
                {
                    if (m == ContentType)
                    {
                        continue;
                    }
                    requestHeaders.Add(m, headers[m]);
                }
            }
            var bodyContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

            if (!string.IsNullOrWhiteSpace(cookies))
            {
                bodyContent.Headers.Add("Cookie", cookies);
            }
            return await httpClient.PostAsync(url, bodyContent);
        }
        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResponse(string url, string contentType = "application/json")
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue(contentType));
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return null;
        }

        public static T GetResponseXML<T>(string url) where T : class, new()
        {
            return GetResponse<T>(url, "application/xml");
        }

        public static T GetResponse<T>(string url, string contentType = "application/json")
            where T : class, new()
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue(contentType));
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            T result = default(T);

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;
                if (contentType == "application/json")
                {
                    result = JsonConvert.DeserializeObject<T>(s);
                }
                else
                {
                    result = XmlDeserialize<T>(s);
                }
            }
            return result;
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string PostResponse(string url, string postData, string contentType = "application/json")
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return null;
        }

        /// <summary>
        /// 发起post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static T PostResponse<T>(string url, string postData, string contentType = "application/json")
            where T : class, new()
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            HttpClient httpClient = new HttpClient();

            T result = default(T);

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;
                if (contentType == "application/json")
                {
                    result = JsonConvert.DeserializeObject<T>(s);
                }
                else
                {
                    result = XmlDeserialize<T>(s);
                }
            }
            return result;
        }

        /// <summary>
        /// V3接口全部为Xml形式，故有此方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T PostXmlResponse<T>(string url, string xmlString, string contentType = "application/xml")
            where T : class, new()
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(xmlString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            HttpClient httpClient = new HttpClient();

            T result = default(T);

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = XmlDeserialize<T>(s);
            }
            return result;
        }

        /// <summary>
        /// 反序列化Xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string xmlString)
            where T : class, new()
        {
            try
            {
                var xns = new XmlSerializerNamespaces();
                XmlSerializer ser = new XmlSerializer(typeof(T));
                xmlString = xmlString.Replace("ax213:", "").Replace(":ax213", "");
                xmlString = xmlString.Replace("ns:", "").Replace(":ns=", "");
                xmlString = xmlString.Replace("xmlns=\"http://websevices.rezin.com.cn\"", "");
                using (StringReader reader = new StringReader(xmlString))
                {
                    return (T)ser.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("XmlDeserialize发生异常：xmlString:" + xmlString + "异常信息：" + ex.Message);
            }

        }

        public static string XmlSerialize<T>(T model) where T : class, new()
        {
            try
            {
                var xns = new XmlSerializerNamespaces();
                XmlSerializer ser = new XmlSerializer(typeof(T));
                StringBuilder sb01 = new StringBuilder();
                using (StringWriter strwriter = new StringWriter(sb01))
                {
                    ser.Serialize(strwriter, model);
                }
                return sb01.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Serialize：xmlString:" + "异常信息：" + ex.Message);
            }
        }

        /// <summary>
        /// get请求,使用正则表达则获取内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResponse(string url, string regexstarttxt, string regexendtxt, string contentType = "application/json")
        {
            var txt = GetResponse(url, contentType);
            return GetValue(txt, regexstarttxt, regexendtxt);
        }

        /// <summary>
        /// 获得字符串中开始和结束字符串中间得值
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="s">开始</param>
        /// <param name="e">结束</param>
        /// <returns></returns> 
        public static string GetValue(string str, string s, string e)
        {
            Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(str).Value;
        }
    }

    public class HttpResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode code { get; set; }

        /// <summary>
        /// 请求状态
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 返回内容
        /// </summary>
        public T content { get; set; }
    }

    public class HttpResult : HttpResult<string>
    {

    }
}
