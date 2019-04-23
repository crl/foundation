using System.Collections.Generic;
using UnityEngine;

namespace gameSDK
{
    [AddComponentMenu("Lingyu/物体循环移动")]
    class ObjectMoveCyc : MonoBehaviour
    {
        public List<GameObject>  includeGameObjects = new List<GameObject>();
        public List<GameObject> excludeGameObjects = new List<GameObject>();

        public Vector3 startPos=Vector3.zero;
        public Vector3 endPos=Vector3.zero;

        public float speed=0;

        protected List<GameObject> objLst = new List<GameObject>();

        protected bool BExcloudeGameObject(GameObject obj_)
        {
            for(int i = 0 ; i < excludeGameObjects.Count ; i ++)
                if (excludeGameObjects[i] == obj_)
                    return true;

            return false;
        }
        void Start()
        {
            objLst.Clear();
            for (int i = 0 ; i < includeGameObjects.Count ; i ++)
                objLst.Add(includeGameObjects[i]);
            for (int i = 0; i < this.transform.childCount; i ++)
            {
                if (BExcloudeGameObject(transform.GetChild(i).gameObject) == false)
                {
                    objLst.Add(transform.GetChild(i).gameObject);
                }
            }
        }

        void Update()
        {
            float step = Time.deltaTime*speed;
            Vector3 dir = (endPos - startPos).normalized*step;
            for (int i = 0; i < objLst.Count; i ++)
            {
                UpdateObject(objLst[i], dir);
            }
        }

        void UpdateObject(GameObject obj_ , Vector3 dir_)
        {
            Vector3 pos = obj_.transform.position + dir_;
            obj_.transform.position = pos;
            if (Vector3.Dot(pos - endPos, startPos - endPos) <= 0)
            {
                //超过了end
                obj_.transform.position += (startPos - endPos);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(startPos, 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(endPos, 0.2f);
        }
    }
}
