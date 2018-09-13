using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QK.Framework.Core.Log
{
    /// <summary>
    /// 日志记录接口
    /// </summary>
    public interface ILog
    {
        Task Write(LogModel model);
    }
}
