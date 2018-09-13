using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QK.Framework.Core.Thread
{
    /// <summary>
    /// 时间管理器
    /// </summary>
    public class TimerManagement
    {
        private ConcurrentDictionary<string, Timer> timerDic = new ConcurrentDictionary<string, Timer>();
        public void Init()
        {

        }
        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="timername">定时器名称</param>
        /// <param name="second1">第一次启动时间（s）</param>
        /// <param name="second2">每次间隔时间</param>
        /// <param name="timerCallback">回调</param>
        public void RegTimer(string timername, decimal second1, decimal second2, TimerCallback timerCallback)
        {
            UnRegTimer(timername);
            Timer t01 = new Timer(timerCallback, timername, (int)(second1 * 1000m), (int)(second2 * 1000));
            timerDic.TryAdd(timername, t01);

        }
        /// <summary>
        /// 删除定时器
        /// </summary>
        /// <param name="timername"></param>
        public void UnRegTimer(string timername)
        {
            if (timerDic.ContainsKey(timername))
            {
                timerDic[timername].Dispose();
                timerDic.TryRemove(timername, out Timer t01);
            }
        }
    }
}
