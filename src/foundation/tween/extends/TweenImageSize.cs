using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    public class TweenImageSize: RFTweenerTask
    {
        public static RFTweenerTask Play(Image target, float duration, Vector2 size,
           Dictionary<string, object> settings = null)
        {
            return new TweenImageSize().play(target, duration, size, settings);
        }

        private RectTransform rect;

        internal RFTweenerTask play(Image target, float duration, Vector2 size,
            Dictionary<string, object> settings = null)
        {

            RectTransform rectTransform = target.GetComponent<RectTransform>();
            Vector2 oldPosition = rectTransform.sizeDelta;
            this.duration = duration;

            startValue = new float[] {oldPosition.x, oldPosition.y};
            endValue = new float[] {size.x, size.y};

            rect = rectTransform;
            n = 2;
            resultValue = new float[n];
            target.gameObject.AddTween(this);
            return this;
        }

        protected override void doUpdate(float[] value)
        {
            if (rect == null)
            {
                return;
            }
            rect.sizeDelta = new Vector2(value[0], value[1]);
        }
    }
}