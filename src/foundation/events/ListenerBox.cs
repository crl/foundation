using System;

namespace foundation
{
    /// <summary>
    /// 侦听者的优先顺序辅助类
    /// </summary>
    public class ListenerBox<T>
    {
        /// <summary>
        ///  代理
        /// </summary>
        public Action<T> listener;

        /// <summary>
        /// 优化级 
        /// </summary>
        public int priority;

        public ListenerBox(Action<T> listener, int priority = 0)
        {
            this.listener = listener;
            this.priority = priority;
        }
    }
}