using System;

namespace foundation
{
    public class ActionNode<T>
    {
        public ActionNode<T> next;

        public ActionNode<T> pre;

        public Action action;

        public T data;

        /// <summary>
        /// 0:将删除;
        /// 1:正在运行
        /// 2:将加入; 
        /// </summary>
        internal NodeActiveState active = NodeActiveState.Runing;

        public int priority = 0;

    }
}