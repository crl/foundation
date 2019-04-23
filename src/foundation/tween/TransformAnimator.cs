using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    public enum TransformStyle
    {
        Once,
        Loop,
        PingPong,
    }

    [AddComponentMenu("Lingyu/TransformAnimator")]
    public class TransformAnimator:MonoBehaviour
    {
        public float duration = 1.0f;
        public float delay = 0;
        public TransformStyle style = TransformStyle.Once;

        public bool hasPosition = false;
        public bool hasPositionX = true;
        public bool syncPosition = false;
        public bool hasPositionY = false;
        public bool hasPositionZ = false;
        public bool isPositionOffset = false;
        public AnimationCurve animationCurvePosX= AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve animationCurvePosY = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve animationCurvePosZ = AnimationCurve.Linear(0, 0, 1, 1);
        protected Vector3 startPosition;
        private Vector3 realEndPosition;
        public Vector3 endPosition;

        public bool hasRotation = false;
        public bool isQuaternionLerp = false;
        public bool isRotationOffset = false;
        public AnimationCurve animationCurveRotation = AnimationCurve.Linear(0, 0, 1, 1);
        protected Vector3 startRotation=Vector3.zero;
        private Vector3 realEndRotation;
        public Vector3 endEuler=Vector3.zero;

        public bool hasScale = false;
        public AnimationCurve animationCurveScale = AnimationCurve.Linear(0, 0, 1, 1);
        protected Vector3 startScale=Vector3.one;
        public Vector3 endScale=Vector3.one;

        public bool hasColor = false;
        public bool isIncludeAll = false;
        public Color endColor=Color.clear;
        protected Color startColor = Color.white;
        public AnimationCurve animationCurveColor= AnimationCurve.Linear(0, 0, 1, 1);

        public Action<TransformAnimator> onStart;
        public Action<TransformAnimator> onComplete;

        private Renderer _renderer;
        private Graphic _graphic;

        private Renderer[] _renderers;
        private Graphic[] _graphics;

        private float _progress = 0;
        public float progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    doProgress(_progress);
                }
            }
        }

        public bool isPlaying
        {
            get { return _isPlaying; }
        }

        protected void OnEnable()
        {
            if (playOnAwake)
            {
                Play();
            }
        }

        protected void OnDisable()
        {
            Stop();
        }

        [ContextMenu("Reset")]
        public virtual void reset()
        {
            progress = 0;

            startPosition = transform.position;
            startRotation = transform.eulerAngles;

            startScale = transform.localScale;
            animationTime = duration;

            if (isPositionOffset)
            {
                realEndPosition = startPosition + endPosition;
            }
            else
            {
                realEndPosition = endPosition;
            }

            if (isRotationOffset)
            {
                realEndRotation = startRotation + endEuler;
            }
            else
            {
                realEndRotation = endEuler;
            }

            if (hasColor)
            {
               _renderer = this.transform.GetComponent<Renderer>();
                if (_renderer != null)
                {
                    this.startColor = _renderer.sharedMaterial.color;
                }
                else
                {
                    _graphic = this.transform.GetComponent<Graphic>();
                    if (_graphic != null)
                    {
                        this.startColor = _graphic.color;
                    }
                }

                if (isIncludeAll)
                {
                    _renderers = this.transform.GetComponentsInChildren<Renderer>();
                    if (_renderers == null)
                    {
                        _graphics = this.transform.GetComponentsInChildren<Graphic>();
                    }
                }
            }
        }

        private Coroutine playCoroutine = null;
        private bool _isPlaying = false;
        private float animationTime;
        public void Play()
        {
            reset();
            
            switch (style)
            {
                case TransformStyle.Once:
                    playCoroutine = StartCoroutine(PlayOnceCoroutine());
                    break;
                case TransformStyle.Loop:
                    playCoroutine = StartCoroutine(PlayLoopCoroutine());
                    break;
                case TransformStyle.PingPong:
                    playCoroutine = StartCoroutine(PlayPingPongCoroutine());
                    break;
            }
        }

        public void Stop()
        {
            if (playCoroutine != null) StopCoroutine(playCoroutine);
            _isPlaying = false;
            playCoroutine = null;
        }

        IEnumerator PlayOnceCoroutine()
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            if (_isPlaying) { yield break; }
            _isPlaying = true;
            if (onStart != null) onStart(this);

            while (progress < 1.0f)
            {
                progress += Time.deltaTime / animationTime;
                yield return null;
            }

            _isPlaying = false;
            progress = 1.0f;
            if (onComplete != null) onComplete(this);
        }

        IEnumerator PlayLoopCoroutine()
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            if (_isPlaying) { yield break; }
            _isPlaying = true;
            if (onStart != null) onStart(this);

            while (true)
            {
                progress += Time.deltaTime / animationTime;
                if (progress > 1.0f)
                {
                    progress -= 1.0f;
                }
                yield return null;
            }
        }

        IEnumerator PlayPingPongCoroutine()
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            if (_isPlaying) { yield break; }
            _isPlaying = true;
            if (onStart != null) onStart(this);
            bool isPositive = true;

            while (true)
            {
                float t = Time.deltaTime / animationTime;

                if (isPositive)
                {
                    progress += t;
                    if (progress > 1.0f)
                    {
                        isPositive = false;
                        progress -= t;
                    }

                }
                else {

                    progress -= t;
                    if (progress < 0.0f)
                    {
                        isPositive = true;
                        progress += t;
                    }
                }

                yield return null;
            }
        }

        private float v;
        private Vector3 newPosition=Vector3.zero;
        private Quaternion newRotation=Quaternion.identity;
        private Vector3 newScale=Vector3.one;
        
        public bool playOnAwake=true;

        protected virtual void doProgress(float progress)
        {
            if (hasPosition)
            {
                if (syncPosition)
                {
                    v = this.animationCurvePosX.Evaluate(progress);
                    newPosition = this.startPosition + (this.realEndPosition - this.startPosition)*v;
                }
                else
                {
                    if (hasPositionX)
                    {
                        v = this.animationCurvePosX.Evaluate(progress);
                        newPosition.x = this.startPosition.x + (this.realEndPosition.x - this.startPosition.x)*v;
                    }
                    else
                    {
                        newPosition.x = this.startPosition.x;
                    }
                    if (hasPositionY)
                    {
                        v = this.animationCurvePosY.Evaluate(progress);
                        newPosition.y = this.startPosition.y + (this.realEndPosition.y - this.startPosition.y)*v;
                    }
                    else
                    {
                        newPosition.y = startPosition.y;
                    }
                    if (hasPositionZ)
                    {
                        v = this.animationCurvePosZ.Evaluate(progress);
                        newPosition.z = this.startPosition.z + (this.realEndPosition.z - this.startPosition.z)*v;
                    }
                    else
                    {
                        newPosition.z = startPosition.z;
                    }
                }
                transform.position = newPosition;
            }

            if (hasRotation)
            {
                v = this.animationCurveRotation.Evaluate(progress);
                transform.rotation = isQuaternionLerp
                    ? Quaternion.Slerp(Quaternion.Euler(startRotation), Quaternion.Euler(realEndRotation), v)
                    : Quaternion.Euler(new Vector3(
                        Mathf.Lerp(startRotation.x, realEndRotation.x, v),
                        Mathf.Lerp(startRotation.y, realEndRotation.y, v),
                        Mathf.Lerp(startRotation.z, realEndRotation.z, v)));
            }

            if (hasScale)
            {
                v = this.animationCurveScale.Evaluate(progress);
                transform.localScale = this.startScale + (this.endScale - this.startScale)*v;
            }

            if (endColor != Color.clear || hasColor)
            {
                Color clr = Color.Lerp(startColor, endColor, this.animationCurveColor.Evaluate(progress));
                if (_renderer != null)
                {
                    _renderer.material.color = clr;
                    if (isIncludeAll)
                    {
                        foreach (Renderer renderer in _renderers)
                        {
                            renderer.material.color = clr;
                        }
                    }
                }else if (_graphic != null)
                {
                    _graphic.color = clr;
                    if (isIncludeAll)
                    {
                        foreach (Graphic graphic in _graphics)
                        {
                            graphic.color = clr;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 复制;
        /// </summary>
        /// <param name="value"></param>
        public void copyFrom(TransformAnimator value)
        {
            this.hasPosition = value.hasPosition;
            this.hasPositionX = value.hasPositionX;
            this.hasPositionY = value.hasPositionY;
            this.hasPositionZ = value.hasPositionZ;
            this.syncPosition = value.syncPosition;

            this.endPosition = value.endPosition;
            this.animationCurvePosX = value.animationCurvePosX;
            this.animationCurvePosY = value.animationCurvePosY;
            this.animationCurvePosZ = value.animationCurvePosZ;

            this.hasRotation = value.hasRotation;
            this.endEuler = value.endEuler;
            this.animationCurveRotation = value.animationCurveRotation;

            this.hasScale = value.hasScale;
            this.endScale = value.endScale;
            this.animationCurveScale = value.animationCurveScale;
        }
    }
}