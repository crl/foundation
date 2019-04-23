using System.Collections.Generic;

namespace foundation
{
    /// <summary>
    /// 简单事件类,主要附带一个数据项
    /// </summary>
    public class EventX : MiEventX
    {
        static private Stack<EventX> sEventPool = new Stack<EventX>();

        public const string START = "start";

        public const string LOCK = "lock";
        public const string UNLOCK = "unLock";

        public const string READY = "ready";

        public const string OPEN = "open";
        public const string CLOSE = "close";
        public const string PAUSE = "pause";
        public const string STOP = "stop";
        public const string PLAY = "play";

        public const string EXIT = "exit";
        public const string ENTER = "enter";
      
        public const string UPDATE = "update";
        public const string ENTER_FRAME = "enterFrame";

        public const string ADDED = "added";
        public const string ADDED_TO_STAGE = "addedToStage";

        public const string REMOVED = "removed";
        public const string REMOVED_FROM_STAGE = "removedFromStage";

        public const string TRIGGERED = "triggered";

        public const string FLATTEN = "flatten";
        public const string RESIZE = "resize";

        public const string REPAINT = "Repaint";

        public const string PROGRESS = "progress";
        public const string CHANGE = "change";
        public const string COMPLETE = "complete";
        public const string CANCEL = "cancel";

        public const string SUCCESS = "success";
        public const string FAILED = "failed";
      
        public const string SCROLL = "scroll";
        public const string SELECT = "select";

        public const string DESTOTY = "destory";
        public const string DISPOSE = "dispose";
        public const string DATA = "data";

        public const string ERROR = "error";

        public const string TIMEOUT = "timeout";

        public const string CONNECTION = "connection";
       
        public const string ITEM_CLICK = "itemClick";
        public const string CLICK = "click";

        public const string FOCUS_IN = "focus_in";
        public const string FOCUS_OUT = "focus_out";

        public const string TOUCH_BEGAN = "touchBegan";
        public const string TOUCH_END = "touchEnd";
        public const string TOUCH_MOVE = "touchMove";

        public const string FIRE = "fire";
        public const string RELOAD = "reload";
        public const string RESTART = "restart";

        public const string RENDER = "render";
        public const string PING = "ping";

        public const string RENDERABLE_CHANGE = "renderable_change";

        public const string MEDIATOR_SHOW = "mediatorShow";
        public const string MEDIATOR_HIDE = "mediatorHide";
        public const string MEDIATOR_READY = "mediatorReady";
        public const string PROXY_READY = "proxyReady";

        public const string ROOT_CREATED = "rootCreated";
        public const string SET_SKIN = "setSkin";
        public const string STATE_CHANGE = "stateChange";
        public static string CLEAR_CACHE = "clearCache";
        public static string DEPEND_READY = "dependReady";
        public static string CLEAR = "clear";

        private IEventDispatcher mCurrentTarget;

        private bool mBubbles;
        private bool mStopsPropagation;
        private bool mStopsImmediatePropagation;
       

        public EventX(string type, object data = null, bool bubbles = false)
            : base(type, data)
        {
            mBubbles = bubbles;
        }

        public void stopPropagation()
        {
            mStopsPropagation = true;
        }

        public void stopImmediatePropagation()
        {
            mStopsPropagation = mStopsImmediatePropagation = true;
        }

        public bool bubbles
        {
            get
            {
                return mBubbles;
            }
        }

        public IEventDispatcher currentTarget
        {
            get
            {
                return mCurrentTarget;
            }
        }

        internal void setCurrentTarget(IEventDispatcher value)
        {
            mCurrentTarget = value;
        }

        internal void setData(object value) { mData = value; }

        internal bool stopsPropagation
        {
            get
            {
                return mStopsPropagation;
            }
        }

        internal bool stopsImmediatePropagation
        {
            get
            {
                return mStopsImmediatePropagation;
            }
        }

      

        public static EventX FromPool(string type, object data = null, bool bubbles = false)
        {
            EventX e;
            if (sEventPool.Count > 0)
            {
                e = sEventPool.Pop();
                e.reset(type, bubbles, data);
                return e;
            }
            else return new EventX(type, data, bubbles);
        }

        public static void ToPool(EventX e)
        {
            if (sEventPool.Count < 100)
            {
                e.mData = e.mTarget = e.mCurrentTarget = null;
                sEventPool.Push(e); // avoiding 'push'
            }
        }

        internal EventX reset(string type, bool bubbles = false, object data = null)
        {
            mType = type;
            mBubbles = bubbles;
            mData = data;
            mTarget = mCurrentTarget = null;
            mStopsPropagation = mStopsImmediatePropagation = false;
            return this;
        }

        public EventX clone()
        {
            return new EventX(type, data, bubbles);
        }

    }
}
