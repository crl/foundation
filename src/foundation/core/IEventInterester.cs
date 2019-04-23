using System;
using System.Collections.Generic;

namespace foundation
{
    /// <summary>
    ///  事件关注者
    /// </summary>
    public interface IEventInterester
    {
        /**
		 *  对哪些事件观注; 
		 * @return 
		 * 
		 */
        Dictionary<string, Action<EventX>> getEventInterests(InjectEventType type);
    }

    public class SimpleInjectEventInterester:IEventInterester
    {
        private Dictionary<InjectEventType, Dictionary<string, Action<EventX>>> _eventInterests;
        public Dictionary<string, Action<EventX>> getEventInterests(InjectEventType type)
        {
            if (_eventInterests == null)
            {
                _eventInterests = new Dictionary<InjectEventType, Dictionary<string, Action<EventX>>>();
                MVCEventAttribute.CollectionEventInterests(this, _eventInterests);
            }
            Dictionary<string, Action<EventX>> e;
            if (_eventInterests.TryGetValue(type, out e) == false)
            {
                e = new Dictionary<string, Action<EventX>>();
                _eventInterests.Add(type, e);
            }
            return e;
        }
    }
}
