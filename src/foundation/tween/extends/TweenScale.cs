using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class TweenScale: RFTweenerTask
    {
        public static RFTweenerTask Play(GameObject target, float duration, Vector3 scale,
            Dictionary<string, object> settings = null)
        {
            return new TweenScale().play(target, duration, scale, settings);
        }

        private GameObject target;

        internal RFTweenerTask play(GameObject target, float duration, Vector3 scale,
            Dictionary<string, object> settings = null)
        {

            Vector3 oldScale = target.transform.localScale;
            this.duration = duration;

            startValue = new float[] {oldScale.x, oldScale.y, oldScale.z};
            endValue = new float[] {scale.x, scale.y, scale.z};


            this.target = target;
            n = 3;
            resultValue = new float[n];
            target.AddTween(this);
            return this;
        }

        protected override void doUpdate(float[] value)
        {
        
            if (target == null)
            {
                return;
            }
            Vector3 v = new Vector3(value[0], value[1], value[2]);
            target.transform.localScale = v;
        }
    }
}