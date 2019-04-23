using foundation;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.module.chatUI
{
    public class TweenSize:RFTweenerTask
    {
        public static RFTweenerTask Play(RectTransform target, Vector2 size, float duration,
            Dictionary<string, object> settings = null)
        {
            return new TweenSize().play(target, duration, size, settings);
        }

        private RectTransform rect;

        internal RFTweenerTask play(RectTransform target, float duration, Vector2 size,
            Dictionary<string, object> settings = null)
        {
            rect = target;
            Vector2 oldPosition = rect.sizeDelta;
            this.duration = duration;

            startValue = new float[] { oldPosition.x, oldPosition.y };
            endValue = new float[] { size.x, size.y };

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