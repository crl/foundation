using UnityEngine;

namespace foundation
{
    public class TransformTweenTask : RFTweenerTask
    {
        private AnimationCurve m_animation;
        private RectTransform _rectTransform;
        private Rotation _rotation;
        private Scaling _scaling;
        private Transform _transform;
        private Translation _translation;
        private TranslationAnchoredPosition _translationAnchoredPosition;

        public TransformTweenTask(Transform tm, float duration, float delay = 0f)
        {
            this._transform = tm;
            this._rectTransform = !(tm is RectTransform) ? null : ((RectTransform) tm);
            this._timer = 0f;
            this.duration = duration;
            this.delay = delay;
        }

        public TransformTweenTask(Transform tm, AnimationCurve animation, float duration, float delay = 0f)
        {
            if (animation == null)
            {
                Debug.LogError("Animation was null!");
            }
            else if (animation.keys.Length < 2)
            {
                Debug.LogError("Animation had less than two keyframes!");
            }
            this._transform = tm;
            this._rectTransform = !(tm is RectTransform) ? null : ((RectTransform) tm);
            this.m_animation = animation;
          
            this._timer = 0f;
            this.duration = duration;
            this.delay = delay;
        }

        public override void reset()
        {
            base.reset();
            if (_translation != null)
            {
                this._translation.start = !this._translation.local
                    ? this._transform.position
                    : this._transform.localPosition;
            }
            if (this._translationAnchoredPosition != null)
            {
                this._translationAnchoredPosition.start = this._rectTransform.anchoredPosition;
            }
            if (this._rotation != null)
            {
                this._rotation.start = !this._rotation.local
                    ? this._transform.rotation
                    : this._transform.localRotation;
            }
            if (this._scaling != null)
            {
                this._scaling.start = !this._scaling.local
                    ? this._transform.lossyScale
                    : this._transform.localScale;
            }
        }

        protected override void doProgress(float progress)
        {
            if (_translation != null)
            {
                this.applyTranslation(progress);
            }
            if (this._translationAnchoredPosition != null)
            {
                this.applyTranslationToAnchoredPos(progress);
            }
            if (this._rotation != null)
            {
                this.applyRotation(progress);
            }
            if (this._scaling != null)
            {
                this.applyScaling(progress);
            }
        }

        private void applyRotation(float progress)
        {
            Quaternion quaternion;
            float valueAt;
            if (this.m_animation != null)
            {
                valueAt = this.m_animation.Evaluate(progress);
            }
            else
            {
                valueAt = this._rotation.easing(0,1,progress);
            }
            if (this._rotation.lazy)
            {
                quaternion = Quaternion.Lerp(this._rotation.start, this._rotation.target, valueAt);
            }
            else
            {
                quaternion =
                    Quaternion.Euler(this._rotation.start.eulerAngles + ((Vector3) (this._rotation.rotateBy*valueAt)));
            }
            Vector3 eulerAngles = quaternion.eulerAngles;
            if (this._rotation.local)
            {
                Vector3 vector2 = this._transform.localRotation.eulerAngles;
                this._transform.localRotation =
                    Quaternion.Euler(new Vector3(!this._rotation.axis[0] ? vector2.x : eulerAngles.x,
                        !this._rotation.axis[1] ? vector2.y : eulerAngles.y,
                        !this._rotation.axis[2] ? vector2.z : eulerAngles.z));
            }
            else
            {
                Vector3 vector3 = this._transform.rotation.eulerAngles;
                this._transform.rotation =
                    Quaternion.Euler(new Vector3(!this._rotation.axis[0] ? vector3.x : eulerAngles.x,
                        !this._rotation.axis[1] ? vector3.y : eulerAngles.y,
                        !this._rotation.axis[2] ? vector3.z : eulerAngles.z));
            }
        }

        private void applyScaling(float progress)
        {
            float valueAt;
            if (this.m_animation != null)
            {
                valueAt = this.m_animation.Evaluate(progress);
            }
            else
            {
                valueAt = this._scaling.easing(0,1,progress);
            }
            Vector3 scale = this._scaling.start + ((Vector3) ((this._scaling.target - this._scaling.start)*valueAt));
            if (this._scaling.local)
            {
                this._transform.localScale =
                    new Vector3(!this._scaling.axis[0] ? this._transform.localScale.x : scale.x,
                        !this._scaling.axis[1] ? this._transform.localScale.y : scale.y,
                        !this._scaling.axis[2] ? this._transform.localScale.z : scale.z);
            }
            else
            {
                scale = new Vector3(!this._scaling.axis[0] ? this._transform.lossyScale.x : scale.x,
                    !this._scaling.axis[1] ? this._transform.lossyScale.y : scale.y,
                    !this._scaling.axis[2] ? this._transform.lossyScale.z : scale.z);
                this._transform.SetWorldScale(scale);
            }
        }

        private void applyTranslation(float progress)
        {
            float valueAt;
            Vector3 zero = Vector3.zero;
            if (this.m_animation != null)
            {
                valueAt = this.m_animation.Evaluate(progress);
            }
            else
            {
                valueAt = this._translation.easing(0,1,progress);
            }
            if (this._translation.algorithm == TranslationAlgorithm.LINEAR)
            {
                zero = this._translation.start +
                       ((Vector3) ((this._translation.target - this._translation.start)*valueAt));
            }
            else if (this._translation.algorithm == TranslationAlgorithm.CATMULL_ROM)
            {
                zero = MathExtendUtils.CatmullRom(valueAt, this._translation.start + this._translation.cp0,
                    this._translation.start, this._translation.target,
                    this._translation.target + this._translation.cp1);
            }
            else if (this._translation.algorithm == TranslationAlgorithm.QUADRATIC_BEZIER)
            {
                zero = MathExtendUtils.QuadraticBezier(valueAt, this._translation.start, this._translation.cp0,
                    this._translation.target);
            }
            else if (this._translation.algorithm == TranslationAlgorithm.CUBIC_BEZIER)
            {
                zero = MathExtendUtils.CubicBezier(valueAt, this._translation.start, this._translation.cp0,
                    this._translation.cp1, this._translation.target);
            }
            else if (this._translation.algorithm == TranslationAlgorithm.QUARTIC_BEZIER)
            {
                zero = MathExtendUtils.QuarticBezier(valueAt, this._translation.start, this._translation.cp0,
                    this._translation.cp1, this._translation.cp2, this._translation.target);
            }
            if (this._translation.local)
            {
                this._transform.localPosition =
                    new Vector3(!this._translation.axis[0] ? this._transform.localPosition.x : zero.x,
                        !this._translation.axis[1] ? this._transform.localPosition.y : zero.y,
                        !this._translation.axis[2] ? this._transform.localPosition.z : zero.z);
            }
            else
            {
                this._transform.position =
                    new Vector3(!this._translation.axis[0] ? this._transform.position.x : zero.x,
                        !this._translation.axis[1] ? this._transform.position.y : zero.y,
                        !this._translation.axis[2] ? this._transform.position.z : zero.z);
            }
        }

        private void applyTranslationToAnchoredPos(float progress)
        {
            float valueAt;
            Vector2 zero = Vector2.zero;
            if (this.m_animation != null)
            {
                valueAt = this.m_animation.Evaluate(progress);
            }
            else
            {
                valueAt = this._translationAnchoredPosition.easing(0,1,progress);
            }
            if (this._translationAnchoredPosition.algorithm == TranslationAlgorithm.LINEAR)
            {
                zero = this._translationAnchoredPosition.start +
                       ((Vector2)
                           ((this._translationAnchoredPosition.target - this._translationAnchoredPosition.start)*
                            valueAt));
            }
            this._rectTransform.anchoredPosition = new Vector2(zero.x, zero.y);
        }


        public void reset(float duration, float delay, EaseAction ease)
        {
            this.reset();
            this.duration = duration;
            this.delay = delay;
            if (this._translation != null)
            {
                this._translation.easing = ease;
            }
            if (this._translationAnchoredPosition != null)
            {
                this._translationAnchoredPosition.easing = ease;
            }
            if (this._rotation != null)
            {
                this._rotation.easing = ease;
            }
            if (this._scaling != null)
            {
                this._scaling.easing = ease;
            }
        }

        public void rotate(Quaternion target, bool locally=true, EaseAction easing = null)
        {
            if (this._timer > 0f)
            {
                Debug.LogError("Cannot modify task after it has been started!");
            }
            else
            {
                this._rotation = new Rotation();
                this._rotation.axis = new bool[] {true, true, true};
                this._rotation.start = !locally ? this._transform.rotation : this._transform.localRotation;
                this._rotation.target = target;
                this._rotation.local = locally;
                this._rotation.easing = easing;
                this._rotation.lazy = true;
            }
        }

        public void rotate(Vector3 rotateBy, bool locally=true, EaseAction easing = null)
        {
            if (this._timer > 0f)
            {
                Debug.LogError("Cannot modify task after it has been started!");
            }
            else
            {
                this._rotation = new Rotation();
                this._rotation.axis = new bool[] {true, true, true};
                this._rotation.start = !locally ? this._transform.rotation : this._transform.localRotation;
                this._rotation.rotateBy = rotateBy;
                this._rotation.local = locally;
                this._rotation.easing = easing;
                this._rotation.lazy = false;
            }
        }

        public void scale(Vector3 target, bool locally, EaseAction easing = null)
        {
            if (this._timer > 0f)
            {
                Debug.LogError("Cannot modify task after it has been started!");
            }
            else
            {
                this._scaling = new Scaling();
                this._scaling.axis = new bool[] {true, true, true};
                this._scaling.start = !locally ? this._transform.lossyScale : this._transform.localScale;
                this._scaling.target = target;
                this._scaling.local = locally;
                this._scaling.easing = easing;
            }
        }

        public void setRotationAxises(bool x, bool y, bool z)
        {
            if (this._rotation.axis == null)
            {
                this._rotation.axis = new bool[] {x, y, z};
            }
            else
            {
                this._rotation.axis[0] = x;
                this._rotation.axis[1] = y;
                this._rotation.axis[2] = z;
            }
        }

        public void setScalingAxises(bool x, bool y, bool z)
        {
            if (this._scaling.axis == null)
            {
                this._scaling.axis = new bool[] {x, y, z};
            }
            else
            {
                this._scaling.axis[0] = x;
                this._scaling.axis[1] = y;
                this._scaling.axis[2] = z;
            }
        }

        public void setTranslationAxises(bool x, bool y, bool z)
        {
            if (this._translation.axis == null)
            {
                this._translation.axis = new bool[] {x, y, z};
            }
            else
            {
                this._translation.axis[0] = x;
                this._translation.axis[1] = y;
                this._translation.axis[2] = z;
            }
        }

        public void translate(Vector3 target, bool locally, EaseAction easing = null)
        {
            if (this._timer > 0f)
            {
                Debug.LogError("Cannot modify task after it has been started!");
            }
            else
            {
                this._translation = new Translation();
                this._translation.axis = new bool[] {true, true, true};
                this._translation.start = !locally ? this._transform.position : this._transform.localPosition;
                this._translation.target = target;
                this._translation.local = locally;
                this._translation.easing = easing;
                this._translation.algorithm = TranslationAlgorithm.LINEAR;
            }
        }

        public void translateToAnchoredPos(Vector2 targetAnchoredPos, EaseAction easing = null)
        {
            if (this._timer > 0f)
            {
                Debug.LogError("Cannot modify task after it has been started!");
            }
            else if (this._rectTransform == null)
            {
                Debug.LogError("No RectTransform specified before attempting to translate to anchored position!");
            }
            else
            {
                this._translationAnchoredPosition = new TranslationAnchoredPosition();
                this._translationAnchoredPosition.start = this._rectTransform.anchoredPosition;
                this._translationAnchoredPosition.target = targetAnchoredPos;
                this._translationAnchoredPosition.easing = easing;
                this._translationAnchoredPosition.algorithm = TranslationAlgorithm.LINEAR;
            }
        }

        public void translateWithCatmullRom(Vector3 target, bool locally, Vector3 catmullRomP0, Vector3 catmullRomP1,
           EaseAction easing = null)
        {
            if (this._timer > 0f)
            {
                Debug.LogError("Cannot modify task after it has been started!");
            }
            else
            {
                this._translation = new Translation();
                this._translation.axis = new bool[] {true, true, true};
                this._translation.start = !locally ? this._transform.position : this._transform.localPosition;
                this._translation.target = target;
                this._translation.local = locally;
                this._translation.easing = easing;
                this._translation.algorithm = TranslationAlgorithm.CATMULL_ROM;
                this._translation.cp0 = catmullRomP0;
                this._translation.cp1 = catmullRomP1;
            }
        }

        public void translateWithCubicBezier(Vector3 target, bool locally, Vector3 bezierP1, Vector3 bezierP2,
            EaseAction easing = null)
        {
            if (this._timer > 0f)
            {
                Debug.LogError("Cannot modify task after it has been started!");
            }
            else
            {
                this._translation = new Translation();
                this._translation.axis = new bool[] {true, true, true};
                this._translation.start = !locally ? this._transform.position : this._transform.localPosition;
                this._translation.target = target;
                this._translation.local = locally;
                this._translation.easing = easing;
                this._translation.algorithm = TranslationAlgorithm.CUBIC_BEZIER;
                this._translation.cp0 = bezierP1;
                this._translation.cp1 = bezierP2;
            }
        }

        public void translateWithQuadraticBezier(Vector3 target, bool locally, Vector3 bezierP1,
            EaseAction easing = null)
        {
            if (this._timer > 0f)
            {
                Debug.LogError("Cannot modify task after it has been started!");
            }
            else
            {
                this._translation = new Translation();
                this._translation.axis = new bool[] {true, true, true};
                this._translation.start = !locally ? this._transform.position : this._transform.localPosition;
                this._translation.target = target;
                this._translation.local = locally;
                this._translation.easing = easing;
                this._translation.algorithm = TranslationAlgorithm.QUADRATIC_BEZIER;
                this._translation.cp0 = bezierP1;
            }
        }

        public void translateWithQuarticBezier(Vector3 target, bool locally, Vector3 bezierP1, Vector3 bezierP2,
            Vector3 bezierP3, EaseAction easing = null)
        {
            if (this._timer > 0f)
            {
                Debug.LogError("Cannot modify task after it has been started!");
            }
            else
            {
                this._translation = new Translation();
                this._translation.axis = new bool[] {true, true, true};
                this._translation.start = !locally ? this._transform.position : this._transform.localPosition;
                this._translation.target = target;
                this._translation.local = locally;
                this._translation.easing = easing;
                this._translation.algorithm = TranslationAlgorithm.QUARTIC_BEZIER;
                this._translation.cp0 = bezierP1;
                this._translation.cp1 = bezierP2;
                this._translation.cp2 = bezierP3;
            }
        }
        public Vector2 targetAnchoredPosition
        {
            get { return this._translationAnchoredPosition.target; }
        }

        public Vector3 targetPosition
        {
            get { return this._translation.target; }
        }

        public Quaternion targetRotation
        {
            get { return this._rotation.target; }
        }

        public Vector3 targetScale
        {
            get { return this._scaling.target; }
        }

        private class Rotation
        {
            public bool[] axis;
            public EaseAction easing;
            public bool lazy;
            public bool local;
            public Vector3 rotateBy;
            public Quaternion start;
            public Quaternion target;
        }

        private class Scaling
        {
            public bool[] axis;
            public EaseAction easing;
            public bool local;
            public Vector3 start;
            public Vector3 target;
        }

        private class Translation
        {
            public TranslationAlgorithm algorithm;
            public bool[] axis;
            public Vector3 cp0;
            public Vector3 cp1;
            public Vector3 cp2;
            public EaseAction easing;
            public bool local;
            public Vector3 start;
            public Vector3 target;
        }

        private enum TranslationAlgorithm
        {
            LINEAR,
            CATMULL_ROM,
            QUADRATIC_BEZIER,
            CUBIC_BEZIER,
            QUARTIC_BEZIER
        }

        private class TranslationAnchoredPosition
        {
            public TranslationAlgorithm algorithm;
            public EaseAction easing;
            public Vector2 start;
            public Vector2 target;
        }
    }

}