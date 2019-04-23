using System;
using System.Collections.Generic;

namespace foundation
{
    public delegate AssetResource RouterResourceDelegate(string url, string uri, string prefix);

    public class AssetsManager
    {
        public static RouterResourceDelegate routerResourceDelegate;
        /// <summary>
        /// 资源列表
        /// </summary>
        private Dictionary<string, AssetResource> _resourceMap = new Dictionary<string, AssetResource>();
        private static Dictionary<LoaderXDataType, Type> resourceTypeMapping = new Dictionary<LoaderXDataType, Type>();
        public AssetsManager()
        {
            if (_instance != null)
            {
                throw new Exception();
            }
            regist<AssetBundleResource>(LoaderXDataType.PREFAB);
            regist<AssetBundleResource>(LoaderXDataType.ASSETBUNDLE);
            regist<InnerAssetResource>(LoaderXDataType.RESOURCE);
            regist<EditorAssetResource>(LoaderXDataType.EDITORRESOURCE);
        }

		static private AssetsManager _instance;
		static public AssetsManager instance()
		{
			if(_instance==null){
				_instance = new AssetsManager();
			}
			return _instance;
		}

        public static void regist<T>(LoaderXDataType type) where T:AssetResource
        {
            Type clz = typeof (T);
            if (resourceTypeMapping.ContainsKey(type))
            {
                resourceTypeMapping.Remove(type);
            }

            resourceTypeMapping.Add(type, clz);
        }

        public static void bindEventHandle(IEventDispatcher resource, Action<EventX> handle, bool isBind= true){
			if(isBind){
				resource.addEventListener(EventX.COMPLETE, handle);
				resource.addEventListener(EventX.FAILED, handle);
            }
            else{
				resource.removeEventListener(EventX.COMPLETE, handle);
				resource.removeEventListener(EventX.FAILED, handle);
            }
		}
		
		public AssetResource findResource(string url)
		{
            if (url == null)
            {
                return null;
            }
            AssetResource res=null;
		    string key = url.ToLower();
            if(_resourceMap.TryGetValue(key,out res))
            {
                return res;
            }

            return null;
		}
		
		private AssetResource _getResource(string url,LoaderXDataType autoCreateType= LoaderXDataType.BYTES){
            AssetResource res =findResource(url);
			if(res==null){
                Type cls = null;

			    if (resourceTypeMapping.TryGetValue(autoCreateType, out cls) == false)
			    {
			        res = new AssetResource(url);
			    }
			    else
			    {
			        res = (AssetResource) Activator.CreateInstance(cls, url);
			    }

			    res.parserType = autoCreateType;
                res.addEventListener(EventX.DISPOSE,resourceDisposeHandle);

			    string key = url.ToLower();
				_resourceMap[key]=res;
			}
			return res;
		}

      

        public static AssetResource getResource(string url, LoaderXDataType autoCreateType = LoaderXDataType.BYTES)
        {
			return instance()._getResource(url, autoCreateType);
		}
		
		public void _dispose(string url)
		{
            AssetResource res = null;
            //DebugX.Log("disposeAsset:"+url);
            url = url.ToLower();
            if(_resourceMap.TryGetValue(url, out res)) {
                _resourceMap.Remove(url);

                res.removeEventListener(EventX.DISPOSE,resourceDisposeHandle);
				res.__dispose();
            }
		}

        public void _release(string uri)
        {
            AssetResource res = null;
            //DebugX.Log("disposeAsset:"+uri);
            uri = uri.ToLower();
            if (_resourceMap.TryGetValue(uri, out res))
            {
                res.release();
            }
        }

        /*private void _clearAll()
        {
            foreach (AssetResource res in _resourceMap.Values)
            {
                res.removeEventListener(EventX.DISPOSE, resourceDisposeHandle);
                res.__dispose();
            }
            _resourceMap.Clear();
        }*/

        public static void dispose(string uri)
        {
            instance()._dispose(uri);
        }

        public static void release(string uri)
        {
            instance()._release(uri);
        }

        private void resourceDisposeHandle(EventX e)
        {
            AssetResource res = e.target as AssetResource;
            res.removeEventListener(EventX.DISPOSE, resourceDisposeHandle);

            string uri = res.url.ToLower();
            //DebugX.Log("assetDisposeAsset:" + uri);
            if (_resourceMap.ContainsKey(uri))
            {
                _resourceMap.Remove(uri);
            }
        }
    }
}
