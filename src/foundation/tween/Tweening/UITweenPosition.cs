//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>  

namespace clayui
{
    [AddComponentMenu("Tween/UITween Position")]
    public class UITweenPosition : UITweener
    {
        public Vector3 from;
        public Vector3 to;

        [HideInInspector] public bool worldSpace = false;

		private RectTransform mTrans;
        //UIRect mRect;

        public RectTransform cachedTransform
        {
            get
            {
                if (mTrans == null) mTrans = transform as RectTransform;
                return mTrans;
            }
        }

        [System.Obsolete("Use 'value' instead")]
        public Vector3 position
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public Vector3 value
        {
            get
            {
                if (cachedTransform != null)
                {
                    return worldSpace ? cachedTransform.position : cachedTransform.anchoredPosition3D;
                }
                else
                {
                    return worldSpace ? transform.position : transform.localPosition;
                }
            }
            set
            {
                if (cachedTransform != null)
                {
                    if (worldSpace) cachedTransform.position = value;
                    else cachedTransform.anchoredPosition3D = value;
                }
                else
                {
                    if (worldSpace) transform.position = value;
                    else transform.localPosition = value;
                }
            }
        }

        private void Awake()
        {
            //mRect = GetComponent<UIRect>();
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from*(1f - factor) + to*factor;
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        public static UITweenPosition Begin(GameObject go, float duration, Vector3 pos)
        {
            UITweenPosition comp = UITweener.Begin<UITweenPosition>(go, duration);
            comp.from = comp.value;
            comp.to = pos;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        public static UITweenPosition Begin(GameObject go, float duration, Vector3 pos, bool worldSpace)
        {
            UITweenPosition comp = UITweener.Begin<UITweenPosition>(go, duration);
            comp.worldSpace = worldSpace;
            comp.from = comp.value;
            comp.to = pos;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        [ContextMenu("Set 'From' to current value")]
        public override void SetStartToCurrentValue()
        {
            from = value;
        }

        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue()
        {
            to = value;
        }

        [ContextMenu("Assume value of 'From'")]
        private void SetCurrentValueToStart()
        {
            value = from;
        }

        [ContextMenu("Assume value of 'To'")]
        private void SetCurrentValueToEnd()
        {
            value = to;
        }

        public override void copyFrom(UITweener value)
        {
            base.copyFrom(value);

            UITweenPosition other = value as UITweenPosition;
            if (other != null)
            {
                this.from = other.from;
                this.to = other.to;
            }
        }
    }
}