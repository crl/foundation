using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    /// <summary>
    /// 触发式 Animator 状态机 抽像行为
    /// </summary>
    public class AnimatorStateMachine : StateMachineBehaviour
    {
        internal AnimatorStateMachineImplement implementer;

        public AnimatorStateMachine()
        {
        }
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (implementer)
            {
                implementer.OnStateEnter(animator, stateInfo, layerIndex);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (implementer)
            {
                implementer.OnStateExit(animator, stateInfo, layerIndex);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (implementer)
            {
                implementer.OnStateUpdate(animator, stateInfo, layerIndex);
            }
        }
    }
}