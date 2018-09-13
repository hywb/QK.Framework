using System;
using System.Collections.Generic;
using System.Text;

namespace QK.Framework.Core.Exceptions
{
    public class QKException : Exception
    {
        public QKException() { }

        public QKException(string msg)
            : base(msg)
        { }

        public QKException(string msg, Exception ex) : base(msg, ex) { }
    }
}
