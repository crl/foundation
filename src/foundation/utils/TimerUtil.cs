using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public enum DeltaTimeType
    {
        DELTA_TIME,
        FIXED_DELTA_TIME,
        UNSCALED_DELTA_TIME
    }

    public class TimerUtil : QueueHandle<float>
    {
        private float pre = 0;
        private float delayTime = 0.5f;
        public static float GetDeltaTime(DeltaTimeType dtType)
        {
            switch (dtType)
            {
                case DeltaTimeType.DELTA_TIME:
                    return Time.deltaTime;

                case DeltaTimeType.FIXED_DELTA_TIME:
                    return Time.fixedDeltaTime;

                case DeltaTimeType.UNSCALED_DELTA_TIME:
                    return Time.unscaledDeltaTime;
            }
            return Time.maximumDeltaTime;
        }


        private TimerUtil(float delayTime = 0.5f)
        {
            this.delayTime = delayTime;
        }

        private void add(Action<float> handler, float delayTime)
        {
            if (delayTime > 0)
            {
                delayTime = Time.realtimeSinceStartup + delayTime;
            }
            ___addHandle(handler, delayTime, true);
            if (len == 1)
            {
                TickManager.Add(render);
            }
        }

        private static TimerUtil time30;
        /// <summary>
        /// 添加重复调用频率函数
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="delayTime">倒计时时间,会带在handler中,代表剩余时间</param>
        public static void Add30(Action<float> handler, float delayTime = -1)
        {
            if (time30 == null)
            {
                time30 = new TimerUtil(0.03f);
            }
            time30.add(handler, delayTime);
        }


        private static TimerUtil time100;
        /// <summary>
        /// 添加重复调用频率函数
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="delayTime">倒计时时间,会带在handler中,代表剩余时间</param>
        public static void Add100(Action<float> handler, float delayTime=-1)
        {
            if (time100 == null)
            {
                time100 = new TimerUtil(0.1f);
            }
            time100.add(handler,delayTime);
        }

        public static void Remove(Action<float> handler)
        {
            if (time30 != null)
            {
                if (time30.___removeHandle(handler))
                {
                    return;
                }
            }
            if (time100 != null)
            {
                if (time100.___removeHandle(handler))
                {
                    return;
                }
            }
            if (time300 != null)
            {
                if (time300.___removeHandle(handler))
                {
                    return;
                }
            }
            if (time500 != null)
            {
                if (time500.___removeHandle(handler))
                {
                    return;
                }
            }
            if (time700 != null)
            {
                if (time700.___removeHandle(handler))
                {
                    return;
                }
            }

            if (time1000 != null)
            {
                if (time1000.___removeHandle(handler))
                {
                    return;
                }
            }
        }

        private static TimerUtil time300;
        /// <summary>
        /// 添加重复调用频率函数
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="delayTime">倒计时时间,会带在handler中,代表剩余时间</param>
        public static void Add300(Action<float> handler, float delayTime=-1)
        {
            if (time300 == null)
            {
                time300 = new TimerUtil(0.3f);
            }
            time300.add(handler,delayTime);
        }

        private static TimerUtil time500;
        /// <summary>
        /// 添加重复调用频率函数
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="delayTime">倒计时时间,会带在handler中,代表剩余时间</param>
        public static void Add500(Action<float> handler, float delayTime=-1)
        {
            if (time500 == null)
            {
                time500 = new TimerUtil(0.5f);
            }

            time500.add(handler,delayTime);
        }


        private static TimerUtil time700;
        /// <summary>
        /// 添加重复调用频率函数
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="delayTime">倒计时时间,会带在handler中,代表剩余时间</param>
        public static void Add700(Action<float> handler, float delayTime=-1)
        {
            if (time700 == null)
            {
                time700 = new TimerUtil(0.7f);
            }
            time700.add(handler,delayTime);
        }

        private static TimerUtil time1000;
        /// <summary>
        /// 添加重复调用频率函数
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="delayTime">倒计时时间,会带在handler中,代表剩余时间</param>
        public static void Add1000(Action<float> handler, float delayTime=-1)
        {
            if (time1000 == null)
            {
                time1000 = new TimerUtil(1.0f);
            }
            time1000.add(handler,delayTime);
        }

        public void render(float deltaTime)
        {
            if (len < 1)
            {
                TickManager.Remove(render);
                return;
            }
            float now = Time.realtimeSinceStartup;
            float dis = now - pre;
            if (dis < delayTime-0.016f)
            {
                return;
            }
            pre =now;

            dispatching = true;
            SignalNode<float> t = firstNode;

            List<SignalNode<float>> temp = GetSignalNodeList();
         
            while (t != null)
            {
                if (t.active == NodeActiveState.Runing)
                {
                    float delta = delayTime;
                    if (t.data != -1)
                    {
                        delta = t.data - pre;
                        if (delta < 0)
                        {
                            ___removeHandle(t.action);
                        }
                    }
                    t.action(delta);
                }
                temp.Add(t);
                t = t.next;
            }
            dispatching = false;

            int l = temp.Count;
            for (int i = 0; i < l; i++)
            {
                SignalNode<float> item = temp[i];
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
    }
}