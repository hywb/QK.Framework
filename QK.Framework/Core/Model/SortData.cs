using System;
using System.Collections.Generic;
using System.Text;

namespace QK.Framework.Core.Model
{
    /// <summary>
    /// 排序
    /// </summary>
    public class SortData
    {
        public string field { get; set; }

        public SortType type { get; set; }

        /// <summary>
        /// 排序优先级 desc
        /// </summary>
        public int level { get; set; }
    }

    public enum SortType
    {
        /// <summary>
        /// 降序
        /// </summary>
        DESC,

        /// <summary>
        /// 升序
        /// </summary>
        ASC
    }
}
