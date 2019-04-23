using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    /// <summary>
    /// 不受时间暂停控制
    /// </summary>
    public class CallLater:QueueAction<float>
    {
        private static CallLater Instance = new CallLater();

        private static Dictionary<string, Action> ActionMap=new Dictionary<string, Action>();
    
        public CallLater()
        {
        }

        /// <summary>
        /// 添加延迟调用函数
        /// </summary>
        /// <param name="handler">延迟调用函数</param>
        /// <param name="delayTime">延迟秒数</param>
        /// <param name="key">替换掉key相同 已有的handler</param>
        public static void Add(Action handler, float delayTime = 0.016f,string key="")
        {
            if (delayTime <= 0)
            {
                handler();
                return;
            }
            if (delayTime < 0.016f)
            {
                delayTime = 0.016f;
            }

            if (string.IsNullOrEmpty(key) == false)
            {
                Action oldHandle;
                if (ActionMap.TryGetValue(key, out oldHandle))
                {
                    Remove(oldHandle);
                    ActionMap[key] = handler;
                }
                else
                {
                    ActionMap.Add(key, handler);
                }
            }

            Instance.add(delayTime, handler);
        }

        /// <summary>
        /// 如果存在就先删除,强制在延迟时间后;
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="delayTime"></param>
        /// <param name="key"></param>
        public static void ForceAdd(Action handler, float delayTime = 0.016f, string key = "")
        {
            if (Has(handler))
            {
                Remove(handler, key);
            }
            Add(handler,delayTime,key);
        }

        public static bool Has(Action handler)
        {
            return Instance.hasHandle(handler);
        }

        public static void Remove(Action handler, string key = "")
        {
            Instance.___removeHandle(handler);
            if (string.IsNullOrEmpty(key) == false)
            {
                ActionMap.Remove(key);
            }
        }
        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                Action oldHandle;
                if (ActionMap.TryGetValue(key, out oldHandle))
                {
                    Remove(oldHandle, key);
                }
            }
        }

        private void add(float delayTime, Action handler)
        {
            ___addHandle(handler, Time.time + delayTime,true);
            if (len>0)
            {
                TickManager.Add(render);
            }else if (firstNode != null)
            {
                TickManager.Add(render);
                DebugX.LogError("callLater 有bug:"+len);
            }
        }

        //private List<Action> tempHandle = new List<Action>();
        private void render(float deltaTime)
        {
            if (len > 0)
            {
                dispatching = true;
                ActionNode<float> t = firstNode;

                List<ActionNode<float>> temp = GetSignalNodeList();
                float now = Time.time;
                while (t != null)
                {
                    if (t.active == NodeActiveState.Runing)
                    {
                        if (now > t.data)
                        {
                            Remove(t.action);
                            t.action();
                            //DebugX.Log("callLater:" + now + ":" + t.data);
                        }
                        
                    }
                    temp.Add(t);
                    t = t.next;
                }
                dispatching = false;

                int l = temp.Count;
                for (int i = 0; i < l; i++)
                {
                    ActionNode<float> item = temp[i];
                    if (item.active == NodeActiveState.ToDoDelete)
                    {
                        _remove(item, item.action);
                    }
                    else if (item.active == NodeActiveState.ToDoAdd)
                    {
                        item.active = NodeActiveState.Runing;
                    }
                }
                Recycle(temp);
            }
            else
            {
                TickManager.Remove(render);
            }
        }
    }
}
