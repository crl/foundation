using System;
namespace foundation
{
    /// <summary>
    /// 可收回
    /// 一般用于对像池，对像想被回收重复利用;
    /// </summary>
    public interface IPoolable : IDisposable
    {
        /// <summary>
        /// 当从池中取出时,做一些唤醒操作;
        /// </summary>
		void poolAwake();
		
		/// <summary>
        ///  就会被回收到池时,这时必须休眠操作;
		/// </summary>
        void poolRecycle();
    }
}
