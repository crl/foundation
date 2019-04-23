using UnityEngine;

namespace foundation
{
    /// <summary>
    /// 专门做平面投影的操作
    /// </summary>
    [ExecuteInEditMode]
    public class PlaneShadowCaster : MonoBehaviour
    {
        public Transform reciever;

        public Material material;
        public void Awake()
        {
            material = GetComponent<Renderer>().material;
        }


        void Update()
        {
            if (reciever == null || material==null)
            {
                return;
            }
            material.SetMatrix("_World2Ground", reciever.worldToLocalMatrix);
            material.SetMatrix("_Ground2World", reciever.localToWorldMatrix);
        }
    }
}