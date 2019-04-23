using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [AddComponentMenu("Lingyu/EventUIMaterialColorMotion")]
    [ExecuteInEditMode]
    public class EventUIMaterialColorMotion : MonoBehaviour
    {
        public string property;
        public Color value;

        private int _propertyInt;
        private Material _material;

        protected void Start()
        {
            UnityEngine.UI.Graphic graphic = GetComponent<Graphic>();
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
                _material.SetColor(_propertyInt, value);
            }

        }
    }

}