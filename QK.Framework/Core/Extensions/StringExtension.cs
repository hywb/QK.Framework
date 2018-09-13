using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QK.Framework.Core.Extensions
{
    /// <summary>
    /// String 扩展信息
    /// </summary>
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static int ToInt(this string str, int d = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str))
                    return d;
                return Convert.ToInt32(str);
            }
            catch
            {
                return d;
            }
        }

        public static string Md5(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            var md5Provider = new MD5CryptoServiceProvider();
            byte[] digest = Encoding.Default.GetBytes(str);
            digest = md5Provider.ComputeHash(digest);
            return BitConverter.ToString(digest).Replace("-", string.Empty);
        }

        /// <summary>
        /// 是否为手机号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPhone(this string str)
        {
            if (str.IsNullOrEmpty())
                return false;

            var reg = new Regex("^(0|86|17951)?(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$");
            return reg.IsMatch(str);
        }

        /// <summary>
        /// 是否为邮箱
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(this string str)
        {
            if (str.IsNullOrEmpty())
                return false;

            //var reg = new Regex(@"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");
            Regex reg = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            return reg.IsMatch(str);
        }

        /// <summary>
        /// 隐藏部分字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="begin"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToHide(this string str, int begin = 4, int length = 3, string txt = "*")
        {
            if (str.IsNullOrEmpty())
                return str;
            if (str.Length < begin + length)
                return str;
            var tmp = "";
            for (var i = 0; i < length; i++)
                tmp += txt;
            return str.Replace(str.Substring(begin, length), tmp);
        }

        /// <summary>
        /// base64字符串转换字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBase64(this string str)
        {
            var bytes = new byte[] { };
            if (str.IsNullOrEmpty())
                return bytes;

            return Convert.FromBase64String(str);
        }

        public static string ToSha1(this string str)
        {
            if (str.IsNullOrEmpty())
                return "";
            var sha1 = SHA1.Create();
            var tmps = Encoding.UTF8.GetBytes(str);
            var bytes = sha1.ComputeHash(tmps);

            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// 判断是否包含中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool HasChinese(this string str)
        {
            var reg = new Regex("[\u4e00-\u9fa5]+");
            if (reg.IsMatch(str))
            {
                return true;
            }
            return false;
        }
    }
}
