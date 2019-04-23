using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    public class TweenColor : RFTweenerTask
    {
        public static RFTweenerTask Play(GameObject target, float duration, Color color,
            Dictionary<string, object> settings = null)
        {
            return new TweenColor().play(target, duration, color, settings);
        }

        private GameObject target;

        public RFTweenerTask play(GameObject target, float duration, Color color,
            Dictionary<string, object> settings = null)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            Color oldColor = renderer.material.color;
            this.duration = duration;
            initBySetting(settings);

            startValue = new float[] {oldColor.r, oldColor.g, oldColor.b, oldColor.a};
            endValue = new float[] {color.r, color.g, color.b, color.a};

            this.target = target;
            n = 4;
            resultValue = new float[n];
            target.AddTween(this);
            return this;
        }

        protected override void doUpdate(float[] value)
        {
            if(target != null)
            {
                Renderer renderer = target.GetComponent<Renderer>();
                Color clr = new Color(value[0], value[1], value[2], value[3]);
                renderer.material.color = clr;
            }
        }
    }

    

    public class TweenUIColor : RFTweenerTask
    {
        private Graphic target;
        public static RFTweenerTask Play(Graphic target, float duration, Color color)
        {
            return new TweenUIColor().play(target,duration,color);
        }
        public static RFTweenerTask PlayAlpha(Graphic target, float duration, float alpha)
        {
            Color oldColor = target.color;
            oldColor.a = alpha;
            return new TweenUIColor().play(target, duration, oldColor);
        }

        public RFTweenerTask play(Graphic target, float duration, Color color)
        {

            Color oldColor = target.color;
            this.duration = duration;

            startValue = new float[] {oldColor.r, oldColor.g, oldColor.b, oldColor.a};
            endValue = new float[] {color.r, color.g, color.b, color.a};

            this.target = target;
            n = 4;
            resultValue = new float[n];
            target.gameObject.AddTween(this);
            return this;
        }

        protected override void doUpdate(float[] value)
        {
            if (target == null)
            {
                return;
            }
            Color clr = new Color(value[0], value[1], value[2], value[3]);
            target.color = clr;
        }
    }
}