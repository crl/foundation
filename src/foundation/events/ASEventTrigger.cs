using System;
using UnityEngine.EventSystems;

namespace foundation
{
    public class ASEventTrigger : EventTrigger,IEventDispatcher
    {
        private EventDispatcher eventDispatcher;
        protected bool isDown = false;
        public bool mouseEnterEnabled = false;
        public object data;
        public ASEventTrigger()
        {
            
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            this.simpleDispatch(MouseEventX.CLICK, eventData);
            base.OnPointerClick(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            isDown = true;

            if (mouseEnterEnabled)
            {
                TickManager.Add(tick);
            }
            this.simpleDispatch(MouseEventX.MOUSE_DOWN, eventData);
            base.OnPointerDown(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            this.simpleDispatch(MouseEventX.MOUSE_OVER, eventData);
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            this.simpleDispatch(MouseEventX.MOUSE_OUT, eventData);
            base.OnPointerExit(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            isDown = false;
            if (mouseEnterEnabled)
            {
                TickManager.Remove(tick);
            }
            this.simpleDispatch(MouseEventX.MOUSE_UP, eventData);
            base.OnPointerUp(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            this.simpleDispatch(MouseEventX.MOUSE_DRAG, eventData);
            base.OnDrag(eventData);
        }

        protected void tick(float deltaTime)
        {
            this.simpleDispatch(MouseEventX.MOUSE_ENTER);
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
            if (eventDispatcher != null)
            {
                eventDispatcher.Dispose();
                eventDispatcher = null;
            }
        }
    }
}