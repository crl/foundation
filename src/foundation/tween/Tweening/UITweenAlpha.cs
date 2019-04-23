//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's alpha. Works with both UI widgets as well as renderers.
/// </summary>

namespace clayui
{
    [AddComponentMenu("Tween/UITween Alpha")]
    public class UITweenAlpha : UITweener
    {
        [Range(0f, 1f)] public float from = 1f;
        [Range(0f, 1f)] public float to = 1f;

        private bool mCached = false;
        private MaskableGraphic mWidget;
        private Material mMat;
        private SpriteRenderer mSr;
        private CanvasGroup canvasGroup;

        private void Cache()
        {
            mCached = true;
            canvasGroup = GetComponent<CanvasGroup>();
            if(canvasGroup!=null)return;
            mWidget = GetComponent<MaskableGraphic>();
            if (mWidget != null) return;
            mSr = GetComponent<SpriteRenderer>();

            if (mWidget == null && mSr == null)
            {
                Renderer ren = GetComponent<Renderer>();
                if (ren != null) mMat = ren.material;
            }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public float value
        {
            get
            {
                if (!mCached) Cache();
                if (canvasGroup != null) return canvasGroup.alpha;
                if (mWidget != null) return mWidget.color.a;
                if (mSr != null) return mSr.color.a;
                return mMat != null ? mMat.color.a : 1f;
            }
            set
            {
                if (!mCached) Cache();

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = value;
                }
                else if (mWidget != null)
                {
                    Color c = mWidget.color;
                    c.a = value;
                    mWidget.color = c;
                }
                else if (mSr != null)
                {
                    Color c = mSr.color;
                    c.a = value;
                    mSr.color = c;
                }
                else if (mMat != null)
                {
                    Color c = mMat.color;
                    c.a = value;
                    mMat.color = c;
                }
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Mathf.Lerp(from, to, factor);
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        public static UITweenAlpha Begin(GameObject go, float duration, float alpha)
        {
            UITweenAlpha comp = UITweener.Begin<UITweenAlpha>(go, duration);
            comp.from = comp.value;
            comp.to = alpha;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        public override void SetStartToCurrentValue()
        {
            from = value;
        }

        public override void SetEndToCurrentValue()
        {
            to = value;
        }

        public override void copyFrom(UITweener value)
        {
            base.copyFrom(value);

            UITweenAlpha other=value as UITweenAlpha;
            if(other!=null)
            {
                this.from = other.from;
                this.to = other.to;
            }
        }
    }
}