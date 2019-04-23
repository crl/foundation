using Slate;
using UnityEngine;
using UnityEngine.Serialization;

namespace foundation
{

    [DisallowMultipleComponent]
    [AddComponentMenu("Lingyu/NpcFollowPath")]
    public class NpcFollowPath : MonoBehaviour
    {
        public TransformStyle style = TransformStyle.Loop;
        public BezierPath path;

        [Range(0, 1.0f)] public float lookOffset = 0.1f;
        [FormerlySerializedAs("time")] [Range(0, 50f)] public float duration = 5.0f;

        [Range(0, 50f)] public float delay = 0;

        [Range(0, 1.0f)] public float startOffsetPercent = 0;

        private float startTime = 0;

        [Range(-1, 10f)] public int repeatCount = -1;
        private bool _isPlaying = false;
        private int playedCount = 0;
        private float _runLookOffset;

        protected virtual void OnEnable()
        {
            init();

            _isPlaying = true;
        }

        protected void init()
        {
            playedCount = 0;
            startTime = Time.time;
            mFactor = startOffsetPercent;
            mDuration = 0;
            _runLookOffset = lookOffset;
            mAmountPerDelta = 1000;
            if (path != null)
            {
                Vector3 v = path.GetPointAt(mFactor);
                transform.position = v;
            }
        }

        private float mDuration;

        public float amountPerDelta
        {
            get
            {
                if (mDuration != duration)
                {
                    mDuration = duration;
                    mAmountPerDelta = Mathf.Abs((duration > 0f) ? 1f / duration : 1000f) * Mathf.Sign(mAmountPerDelta);
                }
                return mAmountPerDelta;
            }
        }

        private float mAmountPerDelta = 1000f;
        private float mFactor = 0f;

        void Update()
        {
            if (_isPlaying == false)
            {
                return;
            }
            if (path == null || (Time.time - startTime) < delay)
            {
                return;
            }
            mFactor += amountPerDelta * Time.deltaTime;

            if (style == TransformStyle.Loop)
            {
                if (mFactor > 1f)
                {
                    mFactor -= Mathf.Floor(mFactor);
                    playedCount++;
                }
            }
            else if (style == TransformStyle.PingPong)
            {
                if (mFactor > 1f)
                {
                    mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
                    mAmountPerDelta = -mAmountPerDelta;
                    _runLookOffset = -_runLookOffset;
                }
                else if (mFactor < 0f)
                {
                    mFactor = -mFactor;
                    mFactor -= Mathf.Floor(mFactor);
                    mAmountPerDelta = -mAmountPerDelta;
                    _runLookOffset = -_runLookOffset;
                    playedCount++;
                }
            }

            Vector3 v = Vector3.Slerp(path.GetPointAt(mFactor), transform.position, 0.1f);
            transform.position = v;

            float nt = mFactor + _runLookOffset;
            if (style == TransformStyle.Loop)
            {
                if (nt > 1.0)
                {
                    nt -= 1.0f;
                }
                else if (nt < 0)
                {
                    nt += 1.0f;
                }
            }
            Vector3 lv = path.GetPointAt(nt);
            Vector3 dir = lv - v;
            if (dir.magnitude > 0.1f)
            {
                Quaternion q = Quaternion.LookRotation(dir);
                transform.rotation = q;
            }


            if (((style == TransformStyle.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f)) ||
                (repeatCount > 0 && playedCount >= repeatCount))
            {
                _isPlaying = false;
            }
        }
    }
}