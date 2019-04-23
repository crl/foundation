using System;

namespace foundation
{
    /// <summary>
    ///  异步接口
    /// </summary>
    public interface IAsync
    {
        bool isReady
        {
            get;
        }

        bool startSync();
		
		
		bool addReayHandle(Action<EventX> handle);
    }
}
