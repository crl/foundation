using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class TweenMovePath : RFTweenerTask
    {
        public static RFTweenerTask Play(GameObject target, float duration, Vector3[] paths,
            Dictionary<string, object> settings = null)
        {
            return new TweenMovePath().play(target, duration, paths, settings);
        }

        private CRSpline paths;
        private GameObject target;

        public RFTweenerTask play(GameObject target, float duration, Vector3[] paths,
            Dictionary<string, object> settings = null)
        {

            Vector3 oldPosition = target.transform.localPosition;
            this.duration = duration;
            startValue = new float[] { oldPosition.x, oldPosition.y, oldPosition.z };
            n = 3;
            endValue=new float[3];
            resultValue=new float[3];

            bool plotStart;
            int offset;
            if (paths[0] != oldPosition)
            {
                plotStart = true;
                offset = 3;
            }
            else
            {
                plotStart = false;
                offset = 2;
            }

            Vector3[] vector3s = new Vector3[paths.Length + offset];
            if (plotStart)
            {
                vector3s[1] = oldPosition;
                offset = 2;
            }
            else
            {
                offset = 1;
            }

            //populate calculate path;
            Array.Copy(paths, 0, vector3s, offset, paths.Length);

            vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
            vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] +
                                            (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

            this.paths = new CRSpline(vector3s);
            this.target = target;
            target.AddTween(this);
            return this;
        }

        protected override void doProgress(float progress)
        {
  
            if (target != null)
            {
                progress = ease(0, 1, progress);
                Vector3 v = paths.Interp(progress);
               //DebugX.Log("v:"+v+" d:"+ percentage);
                target.transform.localPosition = v;
            }
        }
    }
}