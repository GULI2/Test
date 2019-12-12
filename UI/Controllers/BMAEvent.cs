using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace UI.Controllers
{
    public class BMAEvent
    {
        private static Timer _timer;//定时器

        static BMAEvent()
        {
            _timer = new Timer(new TimerCallback(Processor), null, 1000, 5000);
        }

        /// <summary>
        /// 此方法为空，只是起到激活BrnMall事件处理机制的作用
        /// </summary>
        public static void Start() { }

        /// <summary>
        /// 事件处理程序
        /// </summary>
        /// <param name="state">参数对象</param>
        public static void Processor(object state)
        {
            HttpRuntime.Cache.Insert("test", DateTime.Now.ToString());
        }
    }
}
