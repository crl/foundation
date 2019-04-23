using UnityEngine;

namespace foundation
{
    public class EditorAssetResource: AssetResource
    {
        public EditorAssetResource(string url) : base(url)
        {
            
        }

        protected override void _loadImp(int priority = 0, bool progress = false, uint retryCount = 0)
        {
            string eventType = EventX.COMPLETE;


#if UNITY_EDITOR
            _data=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(url);
            if (_data == null)
            {
                _data = Resources.Load(url);
            }
#else
               _data = Resources.Load(url);
#endif


            if (_data == null)
            {
                _data = "_data is null";
                eventType = EventX.FAILED;

                string message = string.Format("加载文件失败,Resources文件夹下不存在:{0} ", url);
                DebugX.LogWarning(message);
                resourceComplete(eventType, message);
            }
            else
            {
                resourceComplete(eventType);
            }

        }
    }
}