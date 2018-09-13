using System;
using System.Collections.Generic;
using System.Text;

namespace QK.Framework.Core.Model
{
    public class Result<T>
    {
        public bool success { get; set; }

        public string msg { get; set; }

        public string id { get; set; }

        public T data { get; set; }
    }

    public class Result : Result<Object>
    {
    }
}
