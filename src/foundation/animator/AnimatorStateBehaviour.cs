using System;
using UnityEngine;

namespace foundation
{
    public abstract class AnimatorStateBehaviour: IAnimatorStateBehaviour
    {
        public static string HashToString(int stateID)
        {
            string value;
            AnimatorStateMachineImplement.GetHashToString(stateID, out value);
            return value;
        }
        public static int StringToHash(string fullPathName)
        {
            return AnimatorStateMachineImplement.AddStringToHash(fullPathName);
        }

        public AnimatorStateMachineImplement stateMachineImplement { get; internal set; }
        protected GameObject owner;
        protected Animator animator;

        public virtual void init(GameObject owner,Animator animator)
        {
            this.owner = owner;
            this.animator = animator;
        }

        public virtual void enter()
        {
        }

        public virtual void exit()
        {
        }

        public virtual void update()
        {
        }

        public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        public virtual void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        public virtual void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }


        public virtual void destory()
        {
            exit();
        }

    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StateUpdateMethod : System.Attribute
    {
        public string state;

        public StateUpdateMethod(string state)
        {
            this.state = state;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StateEnterMethod : System.Attribute
    {
        public string state;

        public StateEnterMethod(string state)
        {
            this.state = state;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StateExitMethod : System.Attribute
    {
        public string state;

        public StateExitMethod(string state)
        {
            this.state = state;
        }
    }


    public interface IAnimatorStateBehaviour
    {
        void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
        void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
        void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
}