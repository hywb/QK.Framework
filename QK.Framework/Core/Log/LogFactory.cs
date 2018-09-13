using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QK.Framework.Core.Log
{
    public class LogFactory
    {
        private static object objlock = new object();
        private static ILog _log = null;

        public static string AppName = "";

        public static ILog Log
        {
            get
            {
                if (_log != null)
                    return _log;
                lock (objlock)
                {
                    _log = new FileLog();
                }

                return _log;
            }
        }

        public static async Task Error(string error)
        {
            var model = new LogModel
            {
                AppName = AppName,
                Message = error,
                Type = LogType.Error,
                DateTime = DateTime.UtcNow,
            };
            await Log.Write(model);
        }

        public static async Task Error(Exception ex)
        {
            var model = new LogModel
            {
                AppName = AppName,
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                Type = LogType.Error,
                Level = 99,
                DateTime = DateTime.UtcNow,
            };
            await Log.Write(model);
        }

        public static async Task Info(string info)
        {
            var model = new LogModel
            {
                AppName = AppName,
                Message = info,
                Type = LogType.Info,
                DateTime = DateTime.UtcNow,
            };
            await Log.Write(model);
        }
    }
}
