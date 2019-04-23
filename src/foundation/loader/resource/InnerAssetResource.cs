using UnityEngine;

namespace foundation
{
    public class InnerAssetResource: AssetResource
    {
        public InnerAssetResource(string url) : base(url)
        {
            
        }

        protected override void _loadImp(int priority = 0, bool progress = false, uint retryCount = 0)
        {
            string eventType = EventX.COMPLETE;
            _data = Resources.Load(url) as GameObject;

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