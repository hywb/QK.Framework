using System;
using System.Collections.Generic;
using System.Text;

namespace QK.Framework.Core.Log
{
    public class LogModel
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public LogType Type { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 日志描述
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DateTime { get; set; }
    }

    public enum LogType
    {
        Error = 0,

        Info = 1
    }
}
