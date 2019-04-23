namespace gameSDK
{
    using foundation;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class UIMateriaFloatMotion : MonoBehaviour
    {
        public bool ignoreTimeScale = false;
        public TransformStyle style = TransformStyle.Once;
        [Range(0f, 10)]
        public float duration = 1.0f;
        [Range(0f, 10)]
        public float delay = 0;

        [Range(-1, 10)]
        public int repeatCount = -1;

        [ObjectSelecter(typeof(Material), "Assets/Filter2D/Materials/UGUIUnlit")] public Material originalMaterial;
        [ObjectSelecter(typeof(Material), "Assets/Filter2D/Materials/UGUIUnlit")] public Material replaceMaterial;

        [HideInInspector] public Color color = Color.white;
        [HideInInspector] public string colorKey;

        [HideInInspector] public float strengthValue=-1;
        [HideInInspector] public string strengthKey;

        [HideInInspector] public float startValue = 0f;
        [HideInInspector] public float endValue = 1.0f;
        [HideInInspector] public string animationKey;

        public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f),
            new Keyframe(1f, 1f, 1f, 0f));

        private Graphic graphic;
        private bool hasProp;
        private float mStartTime = 0;
        private bool _isPlaying = false;
        private float mDuration = 0f;
        private float mAmountPerDelta = 1000f;
        private float mFactor = 0f;
        private int playedCount = 0;
        void Start()
        {
            graphic = GetComponent<Graphic>();
        }

        [ContextMenu("Play")]
        public void Play()
        {
            this.Play(-1);
        }
        public void Play(float duration)
        {
            if (_isPlaying)
            {
                onComplete();
                _isPlaying = false;
            }
            if (duration > 0)
            {
                this.duration = duration;
            }
            playedCount = 0;
            float time = ignoreTimeScale ? Time.unscaledTime : Time.time;
            mStartTime = time + delay;
            mFactor = mDuration = 0;
            mAmountPerDelta = 1000;
            if (graphic != null && replaceMaterial != null)
            {
                graphic.material =new Material(replaceMaterial);
                hasProp = replaceMaterial.HasProperty(animationKey);

                if (string.IsNullOrEmpty(colorKey)==false && replaceMaterial.HasProperty(colorKey))
                {
                    graphic.material.SetColor(colorKey,color);
                }

                if (string.IsNullOrEmpty(strengthKey) == false && replaceMaterial.HasProperty(strengthKey))
                {
                    graphic.material.SetFloat(strengthKey, strengthValue);
                }
            }
            _isPlaying = true;
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            if (_isPlaying)
            {
                onComplete();
            }
        }


        private void onComplete()
        {
            if (graphic != null)
            {
                graphic.material = originalMaterial;
            }
            _isPlaying = false;
        }

        public bool isPlaying
        {
            get { return _isPlaying; }
        }

        protected virtual void doProgress(float progress)
        {
            float v = this.animationCurve.Evaluate(progress);
            if (hasProp)
            {
                graphic.material.SetFloat(animationKey, startValue + v * (endValue - startValue));
            }
        }

        /// <summary>
        /// Amount advanced per delta time.
        /// </summary>

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

        protected void Update()
        {
            if (Application.isPlaying==false)
            {
                return;
            }
            doUpdate(ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        protected void doUpdate(float deltaTime)
        {
            if (_isPlaying == false)
            {
                return;
            }

            float time = ignoreTimeScale ? Time.unscaledTime : Time.time;
            if (time < mStartTime) return;

            mFactor += amountPerDelta * deltaTime;

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
                // Ping-pong style reverses the direction
                if (mFactor > 1f)
                {
                    mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
                    mAmountPerDelta = -mAmountPerDelta;
                }
                else if (mFactor < 0f)
                {
                    mFactor = -mFactor;
                    mFactor -= Mathf.Floor(mFactor);
                    mAmountPerDelta = -mAmountPerDelta;
                    playedCount++;
                }
            }


            if (((style == TransformStyle.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f)) ||
                (repeatCount > 0 && playedCount >= repeatCount))
            {
                mFactor = Mathf.Clamp01(mFactor);
                doProgress(mFactor);
                onComplete();
            }
            else
            {
                doProgress(mFactor);
            }
        }
    }
}