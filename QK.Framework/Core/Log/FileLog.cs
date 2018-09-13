using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using QK.Framework.Core.Extensions;

namespace QK.Framework.Core.Log
{
    public class FileLog : ILog
    {
        public FileLog()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(dir, "log");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public Task Write(LogModel model)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(dir, "log");
            path = Path.Combine(path, DateTime.Now.ToString("yyyyMM"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var filepath = Path.Combine(path, DateTime.Now.ToString("yyyy-MM-dd")+".log");
            return File.WriteAllTextAsync(filepath, GetLogContext(model));
        }
        private string GetLogContext(LogModel model)
        {
            StringBuilder sb01 = new StringBuilder();
            sb01.Append(model.DateTime.ToShortTimeString() + "\t");
            sb01.Append("应用名称:" + model.AppName + ",");
            sb01.Append("[" + model.Type + "][" + model.Level + "]" + model.Message);
            if (!model.Source.IsNullOrEmpty())
            {
                sb01.AppendLine("源[" + model.Source + "]");
            }
            if (!model.StackTrace.IsNullOrEmpty())
            {
                sb01.AppendLine("堆栈[" + model.StackTrace + "]");
            }
            return sb01.ToString();
        }
    }
}
