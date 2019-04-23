using UnityEngine;
using UnityEngine.EventSystems;

namespace foundation
{
    public class MouseScaleEffectMono : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float scale = 0.95f;
        public float time=0.1f;

        public Vector3 defaultScale= Vector3.one;
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            TweenScale.Play(gameObject, time, defaultScale * scale);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
           TweenScale.Play(gameObject, time, defaultScale);
        }
    }
}