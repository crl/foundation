
using System;

namespace foundation
{
    public interface IEventDispatcher : INotifier
    {
        bool addEventListener(string type, Action<EventX> listener, int priority = 0);
        bool hasEventListener(string type);
        bool removeEventListener(string type, Action<EventX> listener);
        bool dispatchEvent(EventX e);

    }
}
