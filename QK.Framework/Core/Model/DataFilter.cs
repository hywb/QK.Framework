using System;
using System.Collections.Generic;
using System.Text;

namespace QK.Framework.Core.Model
{
    /// <summary>
    /// 数据查询条件
    /// </summary>
    public class DataFilter
    {
        /// <summary>
        /// 数据库字段名
        /// </summary>
        public string field { get; set; }

        /// <summary>
        ///  运算符
        ///  l  模糊查询 
        ///  g  时间大于
        ///  e  时间小于
        ///  d  数字等于
        ///  b  数字大于
        ///  s  数字小于
        ///  n  不等于
        ///  od or等于查询
        ///  o  or模糊查询
        /// </summary>
        public string comparison { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
    }
}
