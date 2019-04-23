using UnityEngine;

namespace foundation
{
    class LockRotate:MonoBehaviour
    {
        private Quaternion mR;
        void Start()
        {
            mR = transform.rotation;
        }

        void Update()
        {
            transform.rotation = mR;
        }
    }
}
