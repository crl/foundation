using System;
using UnityEngine;

namespace foundation
{
    [HideInInspector]
    public class MonoEventDispatcher : MonoBehaviour, IEventDispatcher
    {
        private EventDispatcher eventDispatcher;
        public object data;

        protected virtual void OnEnable()
        {
            if (eventDispatcher == null)
            {
                return;
            }
            this.simpleDispatch(EventX.ADDED_TO_STAGE);
        }

        protected virtual void OnDisable()
        {
            if (eventDispatcher == null)
            {
                return;
            }
            this.simpleDispatch(EventX.REMOVED_FROM_STAGE);
        }

        public bool addEventListener(string type, Action<EventX> listener, int priority = 0)
        {
            if (eventDispatcher == null)
            {
                eventDispatcher = new EventDispatcher(this);
            }
            return eventDispatcher.addEventListener(type, listener, priority);
        }

        public bool hasEventListener(string type)
        {
            if (eventDispatcher == null)
            {
                return false;
            }
            return eventDispatcher.hasEventListener(type);
        }

        public bool removeEventListener(string type, Action<EventX> listener)
        {
            if (eventDispatcher == null)
            {
                return false;
            }
            return eventDispatcher.removeEventListener(type, listener);
        }

        public void removeEventListeners(string type)
        {
            if (eventDispatcher == null)
            {
                return;
            }
            eventDispatcher.removeEventListeners(type);
        }

        public bool dispatchEvent(EventX e)
        {
            if (eventDispatcher == null)
            {
                return false;
            }
            return eventDispatcher.dispatchEvent(e);
        }

        public bool simpleDispatch(string type, object data = null)
        {
            if (eventDispatcher == null)
            {
                return false;
            }
            return eventDispatcher.simpleDispatch(type, data);
        }

        protected virtual void OnDestroy()
        {
            this.simpleDispatch(EventX.DESTOTY);
            if (eventDispatcher != null)
            {
                eventDispatcher.Dispose();
                eventDispatcher = null;
            }
        }
    }
}