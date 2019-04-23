using System;
using UnityEngine;

namespace foundation
{
    public class MonoExecuter : MonoBehaviour, IExecute
    {
        public virtual void execute(params object[] args)
        {
        }
    }
}