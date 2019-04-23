using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class TweenMove:RFTweenerTask
    {
        public static RFTweenerTask Play(GameObject target, Vector3 position, float duration,
            Dictionary<string, object> settings = null)
        {
            return new TweenMove().play(target, position, duration,settings);
        }

        private GameObject target;

        public RFTweenerTask play(GameObject target, Vector3 position, float duration,
            Dictionary<string, object> settings = null)
        {

            Vector3 oldPosition = target.transform.localPosition;
            this.duration = duration;

            startValue = new float[] {oldPosition.x, oldPosition.y, oldPosition.z};
            endValue = new float[] {position.x, position.y, position.z};

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

            target.transform.localPosition = v;
        }
    }
}