using  System;
namespace foundation
{
    public class SignalNode<T>
    {
        public SignalNode<T> next;
		
		public SignalNode<T> pre;
		
		public Action<T> action;

        public T data;

        /// <summary>
        /// 0:将删除;
        /// 1:正在运行
        /// 2:将加入; 
        /// </summary>
        internal NodeActiveState active = NodeActiveState.Runing;

		public int priority =0;
	
    }


}
