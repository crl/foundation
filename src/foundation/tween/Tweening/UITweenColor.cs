//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's color.
/// </summary>

namespace clayui
{
    [AddComponentMenu("Tween/UITween Color")]
    public class UITweenColor : UITweener
    {
        public Color from = Color.white;
        public Color to = Color.white;
        public string nameColor = "";
        protected bool mCached = false;
        protected MaskableGraphic mWidget;
        protected Material mMat;
        protected Light mLight;
        protected SpriteRenderer mSr;

        protected MaskableGraphic[] mWidgets;

        public bool isIncludeAll = false;

        protected void Cache()
        {
            mCached = true;
            if (isIncludeAll == true)
            {
                mWidgets = GetComponentsInChildren<MaskableGraphic>(true);
                if (mWidgets.Length > 0) return;
            }
            mWidget = GetComponent<MaskableGraphic>();
            if (mWidget != null) return;

            mSr = GetComponent<SpriteRenderer>();
            if (mSr != null) return;

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		Renderer ren = renderer;
#else
            Renderer ren = GetComponent<Renderer>();
#endif
            if (ren != null)
            {
                mMat = ren.material;
                return;
            }

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		mLight = light;
#else
            mLight = GetComponent<Light>();
#endif
            if (mLight == null) mWidget = GetComponentInChildren<MaskableGraphic>();
        }

        [System.Obsolete("Use 'value' instead")]
        public virtual Color color
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>
        public virtual Material GetMaterial()
        {
            Renderer ren = GetComponent<Renderer>();
            return ren.material;
        }

        public virtual Color value
        {
            get
            {
                if (!mCached) Cache();
                if (isIncludeAll && mWidgets.Length > 0)
                {
                    return mWidgets[0].color;
                }
                if (mWidget != null) return mWidget.color;
                if (mMat != null)
                {
                    if (string.IsNullOrEmpty(nameColor))
                    {
                        return mMat.color;
                    }
                    else
                    {
                        return mMat.GetColor(nameColor);
                    }
                }

                if (mSr != null) return mSr.color;
                if (mLight != null) return mLight.color;
                return Color.black;
            }
            set
            {
                if (!mCached) Cache();
                if (isIncludeAll && mWidgets.Length > 0)
                {
                    for (int i = 0; i < mWidgets.Length; i++)
                    {
                        mWidgets[i].color = value;
                    }
                }
                if (mWidget != null) mWidget.color = value;
                else if (mMat != null)
                {
                    if (string.IsNullOrEmpty(nameColor))
                    {
                        mMat.color = value;
                    }
                    else
                    {
                        mMat.SetColor(nameColor,value);
                    }
                }
                else if (mSr != null) mSr.color = value;
                else if (mLight != null)
                {
                    mLight.color = value;
                    mLight.enabled = (value.r + value.g + value.b) > 0.01f;
                }
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Color.Lerp(from, to, factor);
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        public static UITweenColor Begin(GameObject go, float duration, Color color)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return null;
#endif
            UITweenColor comp = UITweener.Begin<UITweenColor>(go, duration);
            comp.from = comp.value;
            comp.to = color;

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

            UITweenColor other = value as UITweenColor;
            if (other != null)
            {
                this.from = other.from;
                this.to = other.to;
                this.isIncludeAll = other.isIncludeAll;
            }
        }
    }

}