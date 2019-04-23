//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's rotation.
/// </summary>

namespace clayui
{


    [AddComponentMenu("Tween/UITween Rotation")]
    public class UITweenRotation : UITweener
    {
        public Vector3 from;
        public Vector3 to;
        public bool quaternionLerp = false;

        private Transform mTrans;

        public Transform cachedTransform
        {
            get
            {
                if (mTrans == null) mTrans = transform;
                return mTrans;
            }
        }

        [System.Obsolete("Use 'value' instead")]
        public Quaternion rotation
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public Quaternion value
        {
            get { return cachedTransform.localRotation; }
            set { cachedTransform.localRotation = value; }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = quaternionLerp
                ? Quaternion.Slerp(Quaternion.Euler(from), Quaternion.Euler(to), factor)
                : Quaternion.Euler(new Vector3(
                    Mathf.Lerp(from.x, to.x, factor),
                    Mathf.Lerp(from.y, to.y, factor),
                    Mathf.Lerp(from.z, to.z, factor)));
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        public static UITweenRotation Begin(GameObject go, float duration, Quaternion rot)
        {
            UITweenRotation comp = UITweener.Begin<UITweenRotation>(go, duration);
            comp.from = comp.value.eulerAngles;
            comp.to = rot.eulerAngles;

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
            from = value.eulerAngles;
        }

        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue()
        {
            to = value.eulerAngles;
        }

        [ContextMenu("Assume value of 'From'")]
        private void SetCurrentValueToStart()
        {
            value = Quaternion.Euler(from);
        }

        [ContextMenu("Assume value of 'To'")]
        private void SetCurrentValueToEnd()
        {
            value = Quaternion.Euler(to);
        }

        public override void copyFrom(UITweener value)
        {
            base.copyFrom(value);

            UITweenRotation other = value as UITweenRotation;
            if (other != null)
            {
                this.from = other.from;
                this.to = other.to;
                this.quaternionLerp = other.quaternionLerp;
            }
        }
    }
}