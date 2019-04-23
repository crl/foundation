using System;
using UnityEngine;

namespace foundation
{
    public class TweenGeneral : RFTweenerTask
    {
        public static RFTweenerTask Play(GameObject go, float fromValue, float toValue, Action<float> updateAction, float duration)
        {
            return new TweenGeneral().play(go, fromValue, toValue, updateAction, duration);
        }

        private Action<float> updateAction;
        public RFTweenerTask play(GameObject go, float fromValue, float toValue, Action<float> updateAction, float duration)
        {
            this.duration = duration;

            startValue = new float[] { fromValue };
            endValue = new float[] { toValue };

            n = 1;
            this.updateAction = updateAction;
            resultValue = new float[n];
            go.AddTween(this);
            return this;
        }

        protected override void doUpdate(float[] value)
        {
            updateAction(value[0]);
        }
    }
}