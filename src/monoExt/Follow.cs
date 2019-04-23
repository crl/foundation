using UnityEngine;

namespace foundation
{
    [ExecuteInEditMode]
    [AddComponentMenu("Lingyu/Follow")]
    public class Follow:MonoBehaviour
    {
        public bool scale=true;
        public bool rotation=true;
        public bool position = true;

        public Transform target;

        public Vector3 positionOffset = Vector3.zero;
        public Vector3 rotationOffset = Vector3.zero;

        protected virtual void LateUpdate()
        {
            if (target != null)
            {
                if (position)
                {
                    this.transform.position = target.position+ positionOffset;
                }
                if (rotation)
                {
                    if (rotationOffset != Vector3.zero)
                    {
                        this.transform.eulerAngles = target.transform.eulerAngles + rotationOffset;
                    }
                    else
                    {
                        this.transform.rotation = target.rotation;
                    }
                }
              
                if (scale)
                {
                    this.transform.localScale = target.localScale;
                }
            }
        }
    }
}