using UnityEngine;

namespace gameSDK
{
    public class ExcelRefVO
    {
        public string id="";
        public string name="";
        public string prefabPath="";
        public string uri;
        
        public GameObject prefab;

        public GameObject CreateByPrefab()
        {
#if UNITY_EDITOR
            if (prefab == null)
            {
                prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }
            GameObject instance = GameObject.Instantiate(prefab);
            instance.hideFlags=HideFlags.HideAndDontSave;
            return instance;
#else
            return null;
#endif


        }

    }

    public class ExcelMapVO
    {
        public string id = "";
        public string uri = "";
        public string name = "";
    }
}