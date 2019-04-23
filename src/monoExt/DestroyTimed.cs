using UnityEngine;

namespace foundation
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Lingyu/DestroyTimed")]
    public class DestroyTimed: MonoBehaviour
    {
        public float destroyTime = 5;
        // Use this for initialization
        protected virtual void Start()
        {
            Destroy(this.gameObject, destroyTime);
        }

    }
}