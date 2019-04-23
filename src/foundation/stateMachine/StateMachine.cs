using System.Collections.Generic;
using UnityEngine;


namespace foundation
{
    public class StateMachineEventX : EventX
    {
        new public const string CHANGE = "StateMachineEventX_CHANGE";

        public StateMachineEventX(string type, object data = null)
            : base(type, data)
        {
        }
    }

    /// <summary>
    ///  状态机
    ///  抽像不同事物的状态,让它在一定时间内只处于一个状态
    ///  如：场景
    /// </summary>
    public class StateMachine:MonoBehaviour
    {
        protected Dictionary<string, IState> states;
        protected IState _currentState;
        public float updateLimit = 0;
        private float preTime = 0;

        /// <summary>
        /// 状态机;
        /// </summary>
        /// <param name="target">状态机控制的对像;</param>
        public StateMachine()
        {
            states = new Dictionary<string, IState>();
        }

        /// <summary>
        /// 添加不同的状态; 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool addState(IState value)
        {
            if (states.ContainsKey(value.type))
            {
                return false;
            }
            value.stateMachine = this;
            states.Add(value.type, value);
            //states[value.type] = value;
            return true;
        }

        /// <summary>
        ///  删除状态; 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool removeState(IState value)
        {
            if (hasState(value.type) == false)
            {
                return false;
            }
            value.stateMachine = null;
            states.Remove(value.type);
            return true;
        }

        /// <summary>
        /// 是否存在状态; 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool hasState(string type)
        {
            return states.ContainsKey(type);
        }

        /// <summary>
        /// 取得状态 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>		
        public IState getState(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return null;
            }

            IState state;
            if (states.TryGetValue(type, out state))
            {
                return state;
            }
            return null;
        }

        /// <summary>
        /// 设置当前状态 
        /// </summary>
        public string currentState
        {
            get
            {
                if (_currentState == null)
                {
                    return "";
                }
                return _currentState.type;
            }
            set
            {
                IState newState = getState(value);
              
                if (newState == _currentState)
                {
                    DebugX.Log("currentState not change:"+_currentState);
                    return;
                }

                if (_currentState != null)
                {
                    _currentState.removeEventListener(EventX.EXIT, exitHandle);
                    _currentState.sleep();
                    _currentState = null;
                }

                _currentState = newState;

                if (_currentState != null)
                {
                  
                    string type = _currentState.type;
                    if (_currentState.initialized == false)
                    {
                        _currentState.initialize();
                    }

                    _currentState.addEventListener(EventX.EXIT, exitHandle);
                    _currentState.awaken();
                }
                else if(string.IsNullOrEmpty(value)==false)
                {
                    DebugX.Log("未注册SceneState:"+value);
                }

                this.simpleDispatch(StateMachineEventX.CHANGE);
            }
        }


        protected virtual void Update()
        {
            if (Time.time - preTime < updateLimit)
            {
                return;
            }
            preTime = Time.time;

            if (_currentState != null)
            {
                _currentState.update();
            }
        }

        private void exitHandle(EventX e)
        {
            IState target = e.target as IState;
            target.removeEventListener(EventX.EXIT, exitHandle);

            if (target != _currentState)
            {
                return;
            }

            _currentState = null;
            if (string.IsNullOrEmpty(target.nextState) == false)
            {
                currentState = target.nextState;
            }
            else
            {
                currentState = null;
            }
        }
    }
}
