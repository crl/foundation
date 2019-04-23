using UnityEngine;

namespace gameSDK
{
    public class RendererVisiable:MonoBehaviour
    {
        public bool isVisiable = true;

        protected virtual void OnBecameVisible()
        {
            isVisiable = true;
        }

        protected virtual void OnBecameInvisible()
        {
            isVisiable = false;
        }
    }
}