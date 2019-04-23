namespace foundation
{
    /// <summary>
    ///  状态接口(指单个状态)
    /// </summary>
    public interface IState : IEventDispatcher
    {
        /// <summary>
        /// 是否初始化完成 
        /// </summary>
        bool initialized
        {
            get;
        }

        StateMachine stateMachine
        {
            get; set;
        }
        string nextState
        {
            get;
        }

        /// <summary>
        /// 只做一次调用; 
        /// </summary>
        void initialize();

        /// <summary>
        /// 状态标识; 
        /// </summary>

        string type
        {
            get;
        }

        void update();

        /// <summary>
        /// 退出当前状态时; 
        /// </summary>

        void sleep();

        /// <summary>
        /// 进入当前状态; 
        /// </summary>

        void awaken();
    }
}
