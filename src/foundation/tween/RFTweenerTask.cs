using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public delegate float EaseAction(float start, float end, float value);
    public class RFTweenerTask
    {
        /// <summary>
        /// 下一结点
        /// </summary>
        internal RFTweenerTask next;

        /// <summary>
        /// 前一结点
        /// </summary>
        internal RFTweenerTask prev;

        public float delay;
        public float duration;


        /// <summary>
        ///  初始值
        /// </summary>
        protected float[] startValue;
        /// <summary>
        /// 结果
        /// </summary>
        protected float[] endValue;
       
        protected int n = 0;
        /// <summary>
        ///  运算结果
        /// </summary>
        protected float[] resultValue;
        /// <summary>
        /// 具体的更新

        public NodeActiveState state=NodeActiveState.Runing;
        

        private float startTime;
        private float endTime;
        protected float _timer = 0;
        public virtual void reset()
        {
            _timer = 0;
            startTime = Time.time + delay;
            endTime = startTime + duration;
        }

        internal virtual void initBySetting(Dictionary<string, object> settings)
        {
            if (settings == null)
            {
                return;
            }

            object value;
            if (settings.TryGetValue("delay", out value))
            {
                delay = (float)value;
            }
            if (settings.TryGetValue("ease", out value))
            {
                ease = (EaseAction) (value);
            }

            if (settings.TryGetValue("onStart", out value))
            {
                onStart = (Action<RFTweenerTask>)value;
            }
            if (settings.TryGetValue("onComplete", out value))
            {
                onComplete = (Action<RFTweenerTask>)value;
            }
            if (settings.TryGetValue("onCancel", out value))
            {
                onCancel = (Action<RFTweenerTask>)value;
            }
            if (settings.TryGetValue("onProgress", out value))
            {
                onProgress = (Action<RFTweenerTask,float>)value;
            }
        }

        public virtual bool isCompleted
        {
            get { return (this._timer == this.duration); }
        }

        public EaseAction ease = new EaseAction(defaultEasingFunction);
        protected static float defaultEasingFunction(float start, float end, float value)
        {
            end -= start;
            return end*value + start;
        }

        public object userData
        {
            get; set;
        }

        public Action<RFTweenerTask> onStart;
        public Action<RFTweenerTask> onComplete;
        public Action<RFTweenerTask> onCancel;
        public Action<RFTweenerTask, float> onProgress;
        
        public virtual void stop()
        {
            this.state = NodeActiveState.ToDoDelete;
        }

        public virtual void endTween()
        {
            _timer = this.duration;
            doProgress(1);

            if (onComplete != null)
            {
                onComplete(this);
            }

            this.state = NodeActiveState.ToDoDelete;
        }

        public virtual void tick(float now)
        {
            if (startTime > now)
            {
                return;
            }

            if (_timer == 0f)
            {
                if (onStart != null)
                {
                    onStart(this);
                }
            }
            _timer = Mathf.Min(now - startTime, this.duration);
            float progress = (this.duration <= 0f) ? 1f : Mathf.Clamp01(this._timer/this.duration);
            doProgress(progress);

            if (onProgress != null)
            {
                onProgress(this, progress);
            }

            if (isCompleted)
            {
                if (onComplete != null)
                {
                    onComplete(this);
                }
                this.state = NodeActiveState.ToDoDelete;
            }
        }

        protected virtual void doProgress(float progress)
        {
            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    resultValue[i] = ease(startValue[i], endValue[i], progress);
                }
                doUpdate(resultValue);
            }
        }

        protected virtual void doUpdate(float[] resultValue)
        {
        }
    }
}