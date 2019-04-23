//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for all tweening operations.
/// </summary>

namespace clayui
{


    public abstract class UITweener : MonoBehaviour
    {
        /// <summary>
        /// Current tween that triggered the callback function.
        /// </summary>
      
        public static UITweener current;

        public enum Method
        {
            Linear,
            EaseIn,
            EaseOut,
            EaseInOut,
            BounceIn,
            BounceOut,
        }

        public enum Style
        {
            Once,
            Loop,
            PingPong,
        }

        /// <summary>
        /// Tweening method used.
        /// </summary>

        [HideInInspector] public Method method = Method.Linear;

        /// <summary>
        /// Does it play once? Does it loop?
        /// </summary>

        //[HideInInspector]
        public Style style = Style.Once;

        public Action<UITweener> FinishMathod;

        /// <summary>
        /// Optional curve to apply to the tween's time factor value.
        /// </summary>

        //[HideInInspector]
        public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f),
            new Keyframe(1f, 1f, 1f, 0f));

        /// <summary>
        /// Whether the tween will ignore the timescale, making it work while the game is paused.
        /// </summary>

        //[HideInInspector]
        public bool ignoreTimeScale = false;

        /// <summary>
        /// How long will the tweener wait before starting the tween?
        /// </summary>

        //[HideInInspector]
        public float delay = 0f;

        /// <summary>
        /// How long is the duration of the tween?
        /// </summary>

        //[HideInInspector]
        public float duration = 1f;

        /// <summary>
        /// 是否在关闭时 也自动屏蔽效果;
        /// </summary>
        public bool isOnDisableCloseEnabled = false;

        /// <summary>
        /// Whether the tweener will use steeper curves for ease in / out style interpolation.
        /// </summary>

        [HideInInspector] public bool steeperCurves = false;

        /// <summary>
        /// Used by buttons and tween sequences. Group of '0' means not in a sequence.
        /// </summary>

        [HideInInspector] public int tweenGroup = 0;

        public bool isEnableReplay = false;

        /// <summary>
        /// Event delegates called when the animation finishes.
        /// </summary>

        //[HideInInspector]
        //public List<EventDelegate> onFinished = new List<EventDelegate>();

        // Deprecated functionality, kept for backwards compatibility
        [HideInInspector] public GameObject eventReceiver;

        [HideInInspector] public string callWhenFinished;

        private bool mStarted = false;
        private float mStartTime = 0f;
        private float mDuration = 0f;
        private float mAmountPerDelta = 1000f;
        private float mFactor = 0f;

        /// <summary>
        /// Amount advanced per delta time.
        /// </summary>

        public float amountPerDelta
        {
            get
            {
                if (mDuration != duration)
                {
                    mDuration = duration;
                    mAmountPerDelta = Mathf.Abs((duration > 0f) ? 1f/duration : 1000f)*Mathf.Sign(mAmountPerDelta);
                }
                return mAmountPerDelta;
            }
        }

        /// <summary>
        /// Tween factor, 0-1 range.
        /// </summary>

        public float tweenFactor
        {
            get { return mFactor; }
            set { mFactor = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// Direction that the tween is currently playing in.
        /// </summary>

        //public AnimationOrTween.Direction direction { get { return amountPerDelta < 0f ? AnimationOrTween.Direction.Reverse : AnimationOrTween.Direction.Forward; } }

        /// <summary>
        /// This function is called by Unity when you add a component. Automatically set the starting values for convenience.
        /// </summary>

        private void Reset()
        {
            if (!mStarted)
            {
                SetStartToCurrentValue();
                SetEndToCurrentValue();
            }
        }

        /// <summary>
        /// Update as soon as it's started so that there is no delay.
        /// </summary>

        protected virtual void Start()
        {
            Update();
        } 

        protected virtual void OnEnable()
        {
            if (isEnableReplay)
            {
                ResetToBeginning();
                PlayForward();
            }
        }

        /// <summary>
        /// Update the tweening factor and call the virtual update function.
        /// </summary>

        private void Update()
        {
            float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            float time = ignoreTimeScale ? Time.unscaledTime : Time.time;

            if (!mStarted)
            {
                mStarted = true;
                mStartTime = time + delay;
            }

            if (time < mStartTime) return;

            // Advance the sampling factor
            mFactor += amountPerDelta*delta;

            // Loop style simply resets the play factor after it exceeds 1.
            if (style == Style.Loop)
            {
                if (mFactor > 1f)
                {
                    mFactor -= Mathf.Floor(mFactor);
                }
            }
            else if (style == Style.PingPong)
            {
                // Ping-pong style reverses the direction
                if (mFactor > 1f)
                {
                    mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
                    mAmountPerDelta = -mAmountPerDelta;
                }
                else if (mFactor < 0f)
                {
                    mFactor = -mFactor;
                    mFactor -= Mathf.Floor(mFactor);
                    mAmountPerDelta = -mAmountPerDelta;
                }
            }

            // If the factor goes out of range and this is a one-time tweening operation, disable the script
            if ((style == Style.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f))
            {
                mFactor = Mathf.Clamp01(mFactor);
                Sample(mFactor, true);
                if (isEnableReplay == false)
                {
                    enabled = false;
                }

                if (current != this)
                {
                    UITweener before = current;
                    current = this;

                    //if (onFinished != null)
                    //{
                    //mTemp = onFinished;
                    //onFinished = new List<EventDelegate>();

                    //// Notify the listener delegates
                    //EventDelegate.Execute(mTemp);

                    //// Re-add the previous persistent delegates
                    //for (int i = 0; i < mTemp.Count; ++i)
                    //{
                    //	EventDelegate ed = mTemp[i];
                    //	if (ed != null && !ed.oneShot) EventDelegate.Add(onFinished, ed, ed.oneShot);
                    //}
                    //mTemp = null;
                    //}
                    if (FinishMathod != null)
                    {
                        FinishMathod(this);
                    }
                    // Deprecated legacy functionality support
                    if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
                        eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);

                    current = before;
                }
            }
            else Sample(mFactor, false);
        }

        /// <summary>
        /// Mark as not started when finished to enable delay on next play.
        /// </summary>

        private void OnDisable()
        {
            if (isOnDisableCloseEnabled && enabled==true)
            {
                enabled = false;
                ResetToDefault();
            }
            mStarted = false;
        }

        /// <summary>
        /// Sample the tween at the specified factor.
        /// </summary>

        public void Sample(float factor, bool isFinished)
        {
            // Calculate the sampling value
            float val = Mathf.Clamp01(factor);

            if (method == Method.EaseIn)
            {
                val = 1f - Mathf.Sin(0.5f*Mathf.PI*(1f - val));
                if (steeperCurves) val *= val;
            }
            else if (method == Method.EaseOut)
            {
                val = Mathf.Sin(0.5f*Mathf.PI*val);

                if (steeperCurves)
                {
                    val = 1f - val;
                    val = 1f - val*val;
                }
            }
            else if (method == Method.EaseInOut)
            {
                const float pi2 = Mathf.PI*2f;
                val = val - Mathf.Sin(val*pi2)/pi2;

                if (steeperCurves)
                {
                    val = val*2f - 1f;
                    float sign = Mathf.Sign(val);
                    val = 1f - Mathf.Abs(val);
                    val = 1f - val*val;
                    val = sign*val*0.5f + 0.5f;
                }
            }
            else if (method == Method.BounceIn)
            {
                val = BounceLogic(val);
            }
            else if (method == Method.BounceOut)
            {
                val = 1f - BounceLogic(1f - val);
            }

            // Call the virtual update
            OnUpdate((animationCurve != null) ? animationCurve.Evaluate(val) : val, isFinished);
        }

        /// <summary>
        /// Main Bounce logic to simplify the Sample function
        /// </summary>

        private float BounceLogic(float val)
        {
            if (val < 0.363636f) // 0.363636 = (1/ 2.75)
            {
                val = 7.5685f*val*val;
            }
            else if (val < 0.727272f) // 0.727272 = (2 / 2.75)
            {
                val = 7.5625f*(val -= 0.545454f)*val + 0.75f; // 0.545454f = (1.5 / 2.75) 
            }
            else if (val < 0.909090f) // 0.909090 = (2.5 / 2.75) 
            {
                val = 7.5625f*(val -= 0.818181f)*val + 0.9375f; // 0.818181 = (2.25 / 2.75) 
            }
            else
            {
                val = 7.5625f*(val -= 0.9545454f)*val + 0.984375f; // 0.9545454 = (2.625 / 2.75) 
            }
            return val;
        }

        /// <summary>
        /// Play the tween.
        /// </summary>

        //[System.Obsolete("Use PlayForward() instead")]
        //public void Play () { Play(true); }

        /// <summary>
        /// Play the tween forward.
        /// </summary>

        public virtual void PlayForward()
        {
            Play(true);
        }

        /// <summary>
        /// Play the tween in reverse.
        /// </summary>

        public void PlayReverse()
        {
            Play(false);
        }

        /// <summary>
        /// Manually activate the tweening process, reversing it if necessary.
        /// </summary>

        public void Play(bool forward)
        {
            mAmountPerDelta = Mathf.Abs(amountPerDelta);
            if (!forward) mAmountPerDelta = -mAmountPerDelta;
            enabled = true;
            Update();
        }

        /// <summary>
        /// Manually reset the tweener's state to the beginning.
        /// If the tween is playing forward, this means the tween's start.
        /// If the tween is playing in reverse, this means the tween's end.
        /// </summary>

        public void ResetToBeginning(bool updateIt = true)
        {
            mStarted = false;
            mFactor = (amountPerDelta < 0f) ? 1f : 0f;
            if (updateIt)
            {
                Sample(mFactor, false);
            }
        }

        /// <summary>
        /// 强制到正常的起始状态;
        /// </summary>
        public void ResetToDefault(bool updateIt=true)
        {
            mStarted = false;
            mFactor = 0f;
            if (updateIt)
            {
                Sample(mFactor, false);
            }
        }

        /// <summary>
        /// Manually start the tweening process, reversing its direction.
        /// </summary>

        public void Toggle()
        {
            if (mFactor > 0f)
            {
                mAmountPerDelta = -amountPerDelta;
            }
            else
            {
                mAmountPerDelta = Mathf.Abs(amountPerDelta);
            }
            enabled = true;
        }

        /// <summary>
        /// Actual tweening logic should go here.
        /// </summary>

        protected abstract void OnUpdate(float factor, bool isFinished);

        /// <summary>
        /// Starts the tweening operation.
        /// </summary>

        public static T Begin<T>(GameObject go, float duration) where T : UITweener
        {
            T comp = go.GetComponent<T>();

            // Find the tween with an unset group ID (group ID of 0).
            if (comp != null && comp.tweenGroup != 0)
            {
                comp = null;
                T[] comps = go.GetComponents<T>();
                for (int i = 0, imax = comps.Length; i < imax; ++i)
                {
                    comp = comps[i];
                    if (comp != null && comp.tweenGroup == 0) break;
                    comp = null;
                }
            }

            if (comp == null)
            {
                comp = go.AddComponent<T>();

                if (comp == null)
                {
                    //DebugX.LogError("Unable to add " + typeof(T) + " to " + NGUITools.GetHierarchy(go), go);
                    return null;
                }
            }
            comp.mStarted = false;
            comp.duration = duration;
            comp.mFactor = 0f;
            comp.mAmountPerDelta = Mathf.Abs(comp.amountPerDelta);
            comp.style = Style.Once;
            comp.animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
            comp.eventReceiver = null;
            comp.callWhenFinished = null;
            comp.enabled = true;
            return comp;
        }


        public virtual void copyFrom(UITweener value)
        {
            if (value == null)
            {
                return;
            }
            this.style = value.style;
            this.delay = value.delay;
            this.duration = value.duration;
            this.ignoreTimeScale = value.ignoreTimeScale;
            this.animationCurve = value.animationCurve;
            this.isOnDisableCloseEnabled = value.isOnDisableCloseEnabled;
            this.isEnableReplay = value.isEnableReplay;
        }

        /// <summary>
        /// Set the 'from' value to the current one.
        /// </summary>

        public virtual void SetStartToCurrentValue()
        {
        }

        /// <summary>
        /// Set the 'to' value to the current one.
        /// </summary>

        public virtual void SetEndToCurrentValue()
        {
        }
    }
}