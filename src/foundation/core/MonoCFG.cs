using UnityEngine;

namespace foundation
{
    public enum TriggerType
    {
        SKILL,
        STOTY
    }
    public class MonoCFG : MonoBehaviour
    {
        protected static float GetHandleSize(Vector3 pos)
        {
            float handleSize = 1f;
#if UNITY_EDITOR
            handleSize = UnityEditor.HandleUtility.GetHandleSize(pos)*0.5f;
#endif
            return handleSize;
        }

        protected virtual void OnDrawGizmos()
        {
        }

        /// <summary>
        /// 是否已被销毁
        /// </summary>
        protected bool isDisposed
        {
            get;
            private set;
        }
        protected virtual void OnDestroy()
        {
            this.isDisposed = true;
        }
    }
}