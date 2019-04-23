using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class TweenQuaternion : RFTweenerTask
    {
        public static RFTweenerTask Play(GameObject target, float duration, Quaternion quaternionValue,
           Dictionary<string, object> settings = null)
        {
            return new TweenQuaternion().play(target, duration, quaternionValue, settings);
        }

        private Quaternion resultRawValue;
        private Quaternion startRawValue;
        private GameObject target;

        public RFTweenerTask play(GameObject target, float duration, Quaternion quaternionValue,
            Dictionary<string, object> settings = null)
        {
            Quaternion oldPosition = target.transform.localRotation;
            this.duration = duration;

            endValue = new float[] {1};
            startValue = new float[] {0};

            resultRawValue = quaternionValue;
            startRawValue = oldPosition;

            this.target = target;
            n = 1;
            resultValue = new float[n];
            target.AddTween(this);
            return this;
        }

        protected override void doUpdate(float[] value)
        {
            if (target != null)
            {
                target.transform.localRotation = Quaternion.Lerp(startRawValue,
                    resultRawValue, (float) value[0]);
            }
        }
    }
}