using UnityEngine;
using UnityEngine.UI;

namespace gameSDK
{
    [AddComponentMenu("Lingyu/EventUIMaterialFloatMotion")]
    [ExecuteInEditMode]
    public class EventUIMaterialFloatMotion : MonoBehaviour
    {
        public string property;

        public float value;

        private int _propertyInt;
        private Material _material;

        protected void Start()
        {
            Graphic graphic = GetComponent<Graphic>();
            if (graphic)
            {
                _material = graphic.material;
                if (string.IsNullOrEmpty(property) == false)
                {
                    _propertyInt = Shader.PropertyToID(property);
                }
            }
        }

        void OnValidate()
        {
            if (string.IsNullOrEmpty(property) == false)
            {
                _propertyInt = Shader.PropertyToID(property);
            }
        }

        void Update()
        {

            if (_material != null)
            {
                _material.SetFloat(_propertyInt, value);
            }

        }
    }
}