using UnityEngine;

namespace foundation
{
    public class ParticleScalingFixBug:MonoBehaviour
    {
        private Transform parentTransform;
        protected virtual void Start()
        {
            parentTransform = this.GetComponent<Transform>().parent;

            float _scale = Mathf.Sqrt(Mathf.Abs(parentTransform.lossyScale.x));

            Vector3 scale = this.transform.localScale;
            scale.x = _scale;
            scale.y = _scale;
            scale.z = _scale;
            transform.localScale = scale;
        }
    }
}