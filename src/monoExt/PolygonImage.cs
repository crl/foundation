﻿namespace foundation
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonImage : Image
    {
        private PolygonCollider2D _polygon = null;

        private PolygonCollider2D polygon
        {
            get
            {
                if (_polygon == null)
                    _polygon = GetComponent<PolygonCollider2D>();
                return _polygon;
            }
        }

        protected PolygonImage()
        {
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return polygon.OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            transform.localPosition = Vector3.zero;
            float w = (rectTransform.sizeDelta.x * 0.5f) + 0.1f;
            float h = (rectTransform.sizeDelta.y * 0.5f) + 0.1f;
            polygon.points = new Vector2[]
            {
                new Vector2(-w, -h),
                new Vector2(w, -h),
                new Vector2(w, h),
                new Vector2(-w, h)
            };
        }

#endif
    }
}