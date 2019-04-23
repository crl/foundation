using clayui;
using foundation;
using UnityEngine;
using UnityEngine.UI;

namespace gameSDK
{
    public abstract class AbstractCameraController : FoundationBehaviour
    {
        public const string CAMERA_FADE_STEP = "cameraFadeStep";
        public const string CAMERA_FADE_END = "cameraFadeEnd";
        protected abstract void updateTransform(float positionAvg, float rotationAvg);

        private static RawImage _cameraFadeTexture;
        private static RFTweenerTask _tweenControl;

        public static RawImage GetCameraFadeTexure()
        {
            if (_cameraFadeTexture == null)
            {
                _cameraFadeTexture = UIUtils.CreateRawImage("FadeScene");
                _cameraFadeTexture.texture = UIUtils.GetSharedColorTexture(Color.white);
                _cameraFadeTexture.color = new Color(0, 0, 0, 0f);

                RectTransform rectTransform=_cameraFadeTexture.rectTransform;
                rectTransform.anchorMin=Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta=Vector2.zero;
                rectTransform.SetParent(UILocater.CanvasLayer.transform,false);
                rectTransform.SetAsLastSibling();

                _cameraFadeTexture.raycastTarget = false;
            }
            return _cameraFadeTexture;
        }

        private float fadeInTime = 0.2f;
        private float delay = 0.5f;
        public virtual void tweenCameraFade(float fadeOutTime,float fadeInTime,float delay=0.5f)
        {
            if (_tweenControl != null)
            {
                _tweenControl.stop();
            }
            this.fadeInTime = fadeInTime;
            this.delay = delay;
            _tweenControl = TweenUIColor.PlayAlpha(GetCameraFadeTexure(), fadeOutTime, 1.0f);
            _tweenControl.onComplete = tweenCameraFadeStep1;
        }

        protected virtual void tweenCameraFadeStep1(RFTweenerTask o)
        {
            _tweenControl = TweenUIColor.PlayAlpha(GetCameraFadeTexure(), fadeInTime, 0);
            _tweenControl.delay = delay;
            _tweenControl.onComplete = tweenCameraFadeStep2;
            _tweenControl.reset();
            this.simpleDispatch(CAMERA_FADE_STEP);
        }

        protected virtual void tweenCameraFadeStep2(RFTweenerTask o)
        {
            _tweenControl = null;
            this.simpleDispatch(CAMERA_FADE_END);
        }
    }
}