using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class TweenMoveScale : RFTweenerTask
    {
        public static RFTweenerTask Play(GameObject target, float duration,Vector3 position, Vector3 scale,
          Dictionary<string, object> settings = null)
        {
            return new TweenMoveScale().play(target, duration, position, scale, settings);
        }
        private GameObject target;
        internal RFTweenerTask play(GameObject target, float duration, Vector3 position, Vector3 scale,
         Dictionary<string, object> settings = null)
        {
         
            Vector3 oldPosition = target.transform.localPosition;
            Vector3 oldScale = target.transform.localScale;
            this.duration = duration;
         
                startValue = new float[]
                {
                    oldPosition.x, oldPosition.y, oldPosition.z,
                    oldScale.x, oldScale.y, oldScale.z,
                };
                endValue = new float[]
                {
                    position.x, position.y, position.z,
                    scale.x, scale.y, scale.z
                };
              
          
            this.target = target;
            n = 6;
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
            target.transform.localPosition = new Vector3(value[0], value[1], value[2]);
            target.transform.localScale = new Vector3(value[3], value[4], 1);
        }
    }
}