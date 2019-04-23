using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace foundation
{
    public class ResourceLoaderManager:ILoaderFactory
    {
        private VersionLoaderFactory versionLoaderFactory;
        /// <summary>
        /// 正在加载中的加载器
        /// </summary>
        private Dictionary<string, RFLoader> _loadingPool= new Dictionary<string, RFLoader>();
        public ResourceLoaderManager()
        {
            versionLoaderFactory = VersionLoaderFactory.GetInstance();
        }

        public RFLoader getLoader(AssetResource resource)
        {
            RFLoader loader = null;
            string url = resource.url;
            LoaderXDataType parserType = resource.parserType;
            if (_loadingPool.TryGetValue(resource.url, out loader))
            {
                return loader;
            }

            string locaPath = versionLoaderFactory.getLocalPathByURL(url, true);
            if (resource.isForceRemote == false)
            {
                //先验证是否有热更新的资源
                string fullLocalPath = PathDefine.getPersistentLocal(locaPath);
                if (File.Exists(fullLocalPath) == true)
                {
                    loader = new FileStreamLoader(fullLocalPath, url, parserType);
                }
                else
                {
                    fullLocalPath = PathDefine.getStreamingAssetsLocal(locaPath, true);
                    if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        ///ios强制使用WebRequest;
                        loader = new WebRequestLoader(fullLocalPath, parserType);
                    }
                    else
                    {
                        loader = new StreamingAssetsLoader(fullLocalPath, url, parserType);
                    }
                }
            }

            if (loader == null)
            {
                loader = new WebRequestLoader(url, parserType);
                loader.isLocalFile = false;
                if (resource.isForceRemote)
                {
                    loader.postData = resource.postData;
                    loader.timeout = resource.timeout;
                }
            }

            _loadingPool[resource.url] = loader;
            return loader;
        }
    }
}
