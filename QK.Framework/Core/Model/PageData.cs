using System;
using System.Collections.Generic;
using System.Text;

namespace QK.Framework.Core.Model
{
    /// <summary>
    /// 分页返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageData<T>
    {
        public long total { get; set; }

        public IEnumerable<T> data { get; set; }
    }
}
