﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace clayui
{
    [AddComponentMenu("Tween/UITween PositionZ")]
    public class UITweenPositionZ : UITweener
    {

        public float from;
        public float to;

        public float RandomRadius = 0;

        /// <summary>
        /// 随机到的终点偏移量
        /// </summary>
        private float RandomValue = 0;

        private RectTransform mTrans;
        //UIRect mRect;

        public RectTransform cachedTransform
        {
            get
            {
                if (mTrans == null) mTrans = transform as RectTransform;
                return mTrans;
            }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public float value
        {
            get
            {
                if(cachedTransform != null)
                return cachedTransform.anchoredPosition3D.z;
                return transform.localPosition.z;
            }
            set
            {
                if (cachedTransform != null)
                {
                    Vector3 temp = cachedTransform.anchoredPosition3D;
                    temp.z = value;
                    cachedTransform.anchoredPosition3D = temp;
                }
                else
                {
                    Vector3 temp = transform.localPosition;
                    temp.z = value;
                    transform.localPosition = temp;
                }
            }
        }
        protected override void Start()
        {
            RandomValue = UnityEngine.Random.Range(-RandomRadius, RandomRadius);
            base.Start();
        }
        public override void PlayForward()
        {
            RandomValue = UnityEngine.Random.Range(-RandomRadius, RandomRadius);
            base.PlayForward();
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from * (1f - factor) + (to + RandomValue) * factor;
        }

        [ContextMenu("Set 'From' to current value")]
        public override void SetStartToCurrentValue()
        {
            from = value;
        }

        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue()
        {
            to = value;
        }

        [ContextMenu("Assume value of 'From'")]
        private void SetCurrentValueToStart()
        {
            value = from;
        }

        [ContextMenu("Assume value of 'To'")]
        private void SetCurrentValueToEnd()
        {
            value = to;
        }

        public override void copyFrom(UITweener value)
        {
            base.copyFrom(value);

            UITweenPositionZ other = value as UITweenPositionZ;
            if (other != null)
            {
                this.from = other.from;
                this.to = other.to;
                this.RandomRadius = other.RandomRadius;
            }
        }
    }
}
