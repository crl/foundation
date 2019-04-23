using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace foundation
{
    public struct StateLayerChangeData
    {
        public int currentState;
        public int lastState;
        public int layerIndex;
    }
    public class AnimatorStateMachineImplement : MonoBehaviour
    {
        protected static Dictionary<int, string> hash2Name = new Dictionary<int, string>();
        protected static Dictionary<string, int> name2Hash = new Dictionary<string, int>();
       
        protected Dictionary<int, List<Action>> updateMethodMap=new Dictionary<int, List<Action>>();
        protected Dictionary<int, List<Action>> enterMethodMap=new Dictionary<int, List<Action>>();
        protected Dictionary<int, List<Action>> exitMethodMap=new Dictionary<int, List<Action>>();
        protected Dictionary<int, AnimatorStateBehaviour> stateMachineBehaviourMaping = new Dictionary<int, AnimatorStateBehaviour>();
        private StateLayerChangeData stateLayerChangeData = new StateLayerChangeData();

        public bool isDebug = false;
        protected Animator _animator;
        protected AnimatorStateMachine _animatorStateMachine;
        protected GameObject _owner;
        protected int[] _lastStateLayers;
        protected int _stateLayerCout=0;

        /// <summary>
        /// 基础层的状态
        /// </summary>
        protected int _currentStateBaseLayer = int.MinValue;
        protected int _lastStateBaseLayer = int.MinValue;

        public static int AddStringToHash(string fullPathName)
        {
            int hash;
            if (name2Hash.TryGetValue(fullPathName, out hash))
            {
                return hash;
            }
            hash = Animator.StringToHash(fullPathName);
            hash2Name.Add(hash, fullPathName);
            name2Hash.Add(fullPathName, hash);

            return hash;
        }

        public static bool GetHashToString(int hash, out string value)
        {
            return hash2Name.TryGetValue(hash, out value);
        }

        public virtual void implement(Animator animator, GameObject owner)
        {
            this._animator = animator;
            _stateLayerCout = _animator.layerCount;
            _lastStateLayers = new int[_stateLayerCout];

            this._owner = owner;
            refreashAnimatorStateMachine();
        }

        protected virtual void OnEnable()
        {
            refreashAnimatorStateMachine();
        }
        protected virtual void OnDisable()
        {
        }

        protected void refreashAnimatorStateMachine()
        {
            if (_animator)
            {
                _animatorStateMachine = _animator.GetBehaviour<AnimatorStateMachine>();
                if (_animatorStateMachine != null)
                {
                    _animatorStateMachine.implementer = this;
                }
            }
        }

        public void discover()
        {
            var type = this.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod);

            foreach (var method in methods)
            {
                object[] attributes;

                attributes = method.GetCustomAttributes(typeof(StateUpdateMethod), true);
                foreach (StateUpdateMethod attribute in attributes)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        int hash=AddStringToHash(attribute.state);
                        addMethodToStateMap(hash, method, updateMethodMap);
                    }
                }


                attributes = method.GetCustomAttributes(typeof(StateEnterMethod), true);
                foreach (StateEnterMethod attribute in attributes)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        int hash = AddStringToHash(attribute.state);
                        addMethodToStateMap(hash, method, enterMethodMap);
                    }
                }

                attributes = method.GetCustomAttributes(typeof(StateExitMethod), true);
                foreach (StateExitMethod attribute in attributes)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        int hash = AddStringToHash(attribute.state);
                        addMethodToStateMap(hash, method, exitMethodMap);
                    }
                }

            }
        }

        protected void addMethodToStateMap(int hash, MethodInfo method, Dictionary<int, List<Action>> toMap)
        {
            if (exitMethodMap.TryGetValue(hash, out tempStateActionsList) == false)
            {
                tempStateActionsList = new List<Action>();
                exitMethodMap.Add(hash, tempStateActionsList);
            }
            tempStateActionsList.Add(() =>
            {
                method.Invoke(this, null);
            });
        }

        public void addStateMachineBehaviour(string fullPathName, AnimatorStateBehaviour behaviour)
        {
            int hash = AddStringToHash(fullPathName);
            addStateMachineBehaviour(hash, behaviour);
        }

        public bool addStateMachineBehaviour(int fullPathHash, AnimatorStateBehaviour behaviour)
        {
            string fullPathName;
            if (GetHashToString(fullPathHash, out fullPathName))
            {
                AnimatorStateBehaviour stateMachineBehaviour;
                if (stateMachineBehaviourMaping.TryGetValue(fullPathHash, out stateMachineBehaviour))
                {
                    stateMachineBehaviourMaping.Remove(fullPathHash);
                    stateMachineBehaviour.destory();
                }

                if (behaviour != null)
                {
                    stateMachineBehaviourMaping.Add(fullPathHash, behaviour);
                    behaviour.stateMachineImplement = this;
                    behaviour.init(_owner, _animator);
                }
                return true;
            }
            return false;
        }



        private AnimatorStateBehaviour stateMachineBehaviour;
        internal void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateMachineBehaviourMaping.TryGetValue(stateInfo.fullPathHash, out stateMachineBehaviour))
            {
                stateMachineBehaviour.OnStateEnter(animator, stateInfo, layerIndex);
            }
        }
        internal void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateMachineBehaviourMaping.TryGetValue(stateInfo.fullPathHash, out stateMachineBehaviour))
            {
                stateMachineBehaviour.OnStateExit(animator, stateInfo, layerIndex);
            }
        }
        internal void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateMachineBehaviourMaping.TryGetValue(stateInfo.fullPathHash, out stateMachineBehaviour))
            {
                stateMachineBehaviour.OnStateUpdate(animator, stateInfo, layerIndex);
            }
        }


        private List<Action> tempStateActionsList;
        public void Update()
        {
            for (int layer = 0; layer < _stateLayerCout; layer++)
            {
                int _lastState = _lastStateLayers[layer];
                int stateId = _animator.GetCurrentAnimatorStateInfo(layer).fullPathHash;
                if (_lastState != stateId)
                {
                    if (layer == 0)
                    {
                        _currentStateBaseLayer = stateId;
                        _lastStateBaseLayer = _lastState;
                    }
                    if (exitMethodMap.TryGetValue(_lastState, out tempStateActionsList))
                    {
                        foreach (Action action in tempStateActionsList)
                        {
                            action.Invoke();
                        }
                    }
                    if (stateMachineBehaviourMaping.TryGetValue(_lastState, out stateMachineBehaviour))
                    {
                        stateMachineBehaviour.exit();
                    }

                    if (enterMethodMap.TryGetValue(stateId, out tempStateActionsList))
                    {
                        foreach (Action action in tempStateActionsList)
                        {
                            action.Invoke();
                        }
                    }
                    if (stateMachineBehaviourMaping.TryGetValue(stateId, out stateMachineBehaviour))
                    {
                        stateMachineBehaviour.enter();
                    }
                    onStateChangeHandle(stateId, _lastState, layer);

                    stateLayerChangeData.currentState = stateId;
                    stateLayerChangeData.lastState = _lastState;
                    stateLayerChangeData.layerIndex = layer;

                    this.simpleDispatch(EventX.CHANGE, stateLayerChangeData);
                }

                if (updateMethodMap.TryGetValue(stateId, out tempStateActionsList))
                {
                    foreach (Action action in tempStateActionsList)
                    {
                        action.Invoke();
                    }
                }
                if (stateMachineBehaviourMaping.TryGetValue(stateId, out stateMachineBehaviour))
                {
                    stateMachineBehaviour.update();
                }

                _lastStateLayers[layer] = stateId;
            }
        }

        public bool isRunningState(int layer,params int[] stateIDs)
        {
            if (layer>=_stateLayerCout)
            {
                return false;
            }

            int layerCurrentState = _lastStateLayers[layer];
            foreach (int state in stateIDs)
            {
                if (layerCurrentState == state)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void onStateChangeHandle(int newStateID, int lastStateID, int layerIndex)
        {
        }
    }
}