namespace foundation
{
    public abstract class AbstractState:EventDispatcher,IState
    {
        public static bool IsDebug = false;
        protected string _type;

        protected string _nextState;

        /// <summary>
        /// 是否已完成初始化;
        /// </summary>
        private bool _initialized = false;

        public StateMachine stateMachine
        {
            get; set; }
        /// <summary>
        /// 当前状态名称;
        /// </summary>
        /// <param name="type"></param>

        public AbstractState()
        {
        }

        /// <summary>
        /// 是否初始化完成 
        /// </summary>

        public bool initialized
        {
            get
            {
                return _initialized;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>

        public virtual void initialize()
        {
            _initialized = true;
        }

        public virtual void update()
        {
            
        }

        /// <summary>
        /// 当前状态名称; 
        /// </summary>
        public string type
        {
            get
            {
                return _type;
            }
        }

        public string nextState
        {
            get
            {
                return _nextState;
            }
            set { _nextState = value; }
        }

        public virtual void sleep()
        {
            if (IsDebug)
            {
                DebugX.Log("sleep:" + type);
            }
            this.simpleDispatch(EventX.EXIT);
        }

        /// <summary>
        /// 进入当前状态; 
        /// </summary>

        public virtual void awaken()
        {
            if (IsDebug)
            {
                DebugX.Log("awaken:" + type);
            }
        }
    }
}
