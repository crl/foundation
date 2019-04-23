using System;
using UnityEngine;

namespace foundation
{
    public class AutoNetWorkListener
    {
        public static float TIME = 3.0f;
        protected static Action ReachableAction;
        public static void Start(Action action)
        {
            ReachableAction = action;
            CallLater.Add(CheckNetWork, TIME);
        }

        private static void CheckNetWork()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Action oldAction = ReachableAction;
                if (oldAction != null)
                {
                    ReachableAction = null;
                    oldAction();
                }
            }
            else
            {
                CallLater.Add(CheckNetWork, TIME);
            }
        }
    }
}