using System;
using System.Collections.Generic;
using System.Text;

namespace QK.Framework.Core.Extensions
{
    /// <summary>
    /// 日期扩展
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// 毫秒时间戳转换日期
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public static DateTime MillisecondsTsToDate(this long times)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(times);
        }

        /// <summary>
        /// 秒时间戳转换日期
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public static DateTime SecondsTsToDate(this long times)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(times);
        }

        /// <summary>
        /// 秒时间戳转换日期
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public static DateTime SecondsTsToDate(this decimal times)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((double)times);
        }

        /// <summary>
        /// 毫秒时间戳转换日期
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public static DateTime MillisecondsTsToDate(this decimal times)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds((double)times);
        }

        /// <summary>
        /// 日期转换为字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToLocalString(this DateTime dt, string format = "yyyy-MM-dd HH:mm:ss")
        {
            if (dt == DateTime.Parse("0001-01-01") || dt == DateTime.Parse("1990-01-01"))
                return "-";
            return dt.AddHours(8).ToString(format);
        }
        public static string ToIntervalDesc(this DateTime dt)
        {
            if (dt == DateTime.Parse("0001-01-01") || dt == DateTime.Parse("1990-01-01"))
                return "-";
            var dt02 = dt.ToLocalTime();
            return GetTimeDiffTxt(dt02);
        }        

        /// <summary>
        /// 获取时间差描述
        /// </summary>
        /// <param name="dt02"></param>
        /// <returns></returns>
        private static string GetTimeDiffTxt(DateTime dt02)
        {
            DateTime dt01 = DateTime.Now;
            var ts01 = dt01 - dt02;
            if (ts01.TotalDays > 10)
            {
                //return (int)(ts01.TotalDays / 365.0) + "年前";
                return dt02.ToLocalString();
            }
            else if (ts01.TotalDays >= 1)
            {
                return (int)(ts01.TotalDays) + "天前";
            }
            else if (ts01.TotalHours >= 1)
            {
                return (int)(ts01.TotalHours) + "小时前";
            }
            else if (ts01.TotalMinutes >= 1)
            {
                return (int)(ts01.TotalMinutes) + "分钟前";
            }
            else
            {
                return "刚刚";
            }
        }
    }
}
