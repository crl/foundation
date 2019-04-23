using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class AssetBundleResource : AssetResource
    {
        protected static Dictionary<string, string> manifesMaping = new Dictionary<string, string>();

        protected float currentProgress = 0;
        public static void RegistMapping(string pathKey, string manifesKey)
        {
            if (manifesMaping.ContainsKey(pathKey))
            {
                manifesMaping.Remove(pathKey);
            }
            manifesMaping.Add(pathKey, manifesKey);
        }

        protected string manifesKey = "";
        protected string manifesPrefix;
        protected string dependKey;
        protected bool isProgress = false;
        protected AssetBundleManifest assetBundleManifest;

        protected List<AssetResource> dependenciesResource = new List<AssetResource>();
        protected AssetResource manifestResource;

        public AssetBundleResource(string url) : base(url)
        {
            foreach (string key in manifesMaping.Keys)
            {
                if (url.IndexOf(key) != -1)
                {
                    manifesKey = manifesMaping[key];
                }
            }
        }

        protected override void _loadImp(int priority = 0, bool progress = false, uint retryCount = 0)
        {
            this.isProgress = progress;
            string manifesURI = "/" + manifesKey + "/";
            int len = manifesURI.Length;

            if (string.IsNullOrEmpty(manifesKey))
            {
                throw new Exception("不正常:" + _url + "\tmanifesKey:" + manifesKey);
            }

            int index = _url.IndexOf(manifesURI);
            manifesPrefix = _url.Substring(0, index + len);
            dependKey = url.Substring(index + len).ToLower();
            //prefab会被转换成小写的;
            this._url = manifesPrefix + dependKey;
            string manifesURL = manifesPrefix + manifesKey;
            //不做除引用操作;

            manifestResource = AssetsManager.getResource(manifesURL, LoaderXDataType.MANIFEST);
            manifestResource.initManifes(manifesPrefix, manifesKey);
            manifestResource.isForceRemote = isForceRemote;
            AssetsManager.bindEventHandle(manifestResource, manifesHandle);
            manifestResource.load();
        }

        protected override void progressHandle(EventX e)
        {
            simpleDispatch(e.type, (float) e.data * (1f - currentProgress) + currentProgress);
        }

        private void manifesHandle(EventX e)
        {
            currentProgress = 0.1f;
            simpleDispatch(EventX.PROGRESS, currentProgress);
            if (dependenciesResource.Count > 0)
            {
                foreach (AssetResource assetResource in dependenciesResource)
                {
                    assetResource.removeEventListener(EventX.DEPEND_READY, dependsHandle);
                    AssetsManager.bindEventHandle(assetResource, dependsHandle, false);
                    assetResource.release();
                }
                dependenciesResource.Clear();
            }

            AssetResource resource = e.target as AssetResource;
            AssetsManager.bindEventHandle(resource, manifesHandle, false);
            if (e.type != EventX.COMPLETE)
            {
                _data = null;
                resourceComplete(EventX.FAILED);
                return;
            }
            assetBundleManifest = resource.data as AssetBundleManifest;

            string[] dependencies = assetBundleManifest.GetDirectDependencies(dependKey);
            AssetResource tempResource;
            int len = _needLoadedDependCount = dependencies.Length;
            //增加自身
            _needLoadedDependCount += 1;

            if (len > 0)
            {
                //DebugX.Log(dependKey + ":::::::::::::::::::::::::::::::::::::" + len);
                for (int i = 0; i < len; i++)
                {
                    string dependency = dependencies[i];
                    if (dependency == dependKey)
                    {
                        _needLoadedDependCount--;
                        continue;
                    }
                    string url = manifesPrefix + dependency;
                    //DebugX.Log(url);
                    tempResource = AssetsManager.getResource(url, LoaderXDataType.ASSETBUNDLE);
                    if (dependenciesResource.Contains(tempResource))
                    {
                        DebugX.Log("hasDuplicate:" + url);
                        _needLoadedDependCount--;
                        continue;
                    }
                    tempResource.retain();
                    dependenciesResource.Add(tempResource);

                    AssetBundle old;
                    string key = url.ToLower();
                    RFLoader.assetBundleMapping.TryGetValue(key, out old);
                    if (old != null || tempResource.isLoaded)
                    {
                        //DebugX.LogWarning("has:"+url);
                        _needLoadedDependCount--;
                        continue;
                    }

                    tempResource.addEventListener(EventX.DEPEND_READY, dependsHandle);
                    AssetsManager.bindEventHandle(tempResource, dependsHandle);

                    tempResource.load();
                }
            }
      
            _totalDependCount = _needLoadedDependCount;

            base._loadImp(0, isProgress);
        }

        private int _needLoadedDependCount = 0;
        private int _totalDependCount = 0;

        private void dependsHandle(EventX e)
        {
            currentProgress = 0.8f;
            float progress= (_totalDependCount - _needLoadedDependCount) / (float)_totalDependCount;
            simpleDispatch(EventX.PROGRESS, 0.7f * progress + 0.1f);

            AssetResource tempResource = e.target as AssetResource;
            AssetsManager.bindEventHandle(tempResource, dependsHandle, false);
            tempResource.removeEventListener(EventX.DEPEND_READY, dependsHandle);

            _needLoadedDependCount--;
            checkAllComplete();
        }

        protected override void loadComplete(EventX e)
        {
            _needLoadedDependCount--;
            loader = e.target as RFLoader;
            AssetsManager.bindEventHandle(loader, loadComplete, false);
            loader.removeEventListener(EventX.CANCEL, loadCancelHandle);
            if (isProgress)
            {
                loader.removeEventListener(EventX.PROGRESS, progressHandle);
            }

            string eventType = e.type;
            if (eventType != EventX.COMPLETE)
            {
                _data = null;
                resourceComplete(EventX.FAILED, (string) e.data);
                return;
            }

            AssetBundle assetBundle = e.data as AssetBundle;
            if (assetBundle != null)
            {
                _data = assetBundle;
                this.simpleDispatch(EventX.DEPEND_READY);
                //DebugX.Log("dd:"+_url+":"+_data);
                checkAllComplete();
                return;
            }
            else
            {
                _data = null;
                resourceComplete(EventX.FAILED, "assetBundle is null");
            }
        }

        private float preProgress = 0;

        private void checkAllComplete()
        {
            if (_needLoadedDependCount != 0)
            {
                if (_needLoadedDependCount < 0)
                {
                    DebugX.LogError("AssetBundle lenError:" + _needLoadedDependCount + " url:" + url);
                    resourceComplete(EventX.COMPLETE);                   
                }
                return;
            }

            if (isAutoMainAsset == false)
            {
                resourceComplete(EventX.COMPLETE);
                return;
            }
            if (_data is AssetBundle == false)
            {
                resourceComplete(EventX.COMPLETE);
                return;
            }
            AssetBundle assetBundle = (AssetBundle) _data;
            if (assetBundle.isStreamedSceneAssetBundle)
            {
                resourceComplete(EventX.COMPLETE);
                return;
            }

            string name = GetPrefabNameyDependKey(dependKey);
//            if (RFLoader.USE_ASYNC)
//            {
//                BaseApp.Instance.StartCoroutine(loadAssetAsync(assetBundle,name));
//                return;
//            }
            _asset = assetBundle.LoadAsset(name);
            resourceComplete(EventX.COMPLETE);
        }

        private IEnumerator loadAssetAsync(AssetBundle assetBundle, string name)
        {
            AssetBundleRequest request = assetBundle.LoadAssetAsync(name);
            while (request.isDone == false)
            {
                if (isProgress)
                {
                    float t = request.progress * 0.2f + 0.8f;
                    if (t != preProgress)
                    {
                        preProgress = t;
                        simpleDispatch(EventX.PROGRESS, t);
                    }
                }
                yield return null;
            }
            _asset = request.asset;
            resourceComplete(EventX.COMPLETE);
        }

        private UnityEngine.Object _asset;

        public override object getMainAsset()
        {
            if (_asset == null)
            {
                if (_data is AssetBundle)
                {
                    string name = GetPrefabNameyDependKey(_url);
                    _asset = ((AssetBundle) _data).LoadAsset(name);
                }
            }
            return _asset;
        }

        public override object getNewInstance()
        {
            object result = null;
            _asset = (UnityEngine.Object)getMainAsset();
            if (_asset == null)
            {
                return result;
            }

            try
            {
                if (isShaderFinded == false)
                {
                    isShaderFinded = true;
                    RenderUtils.ShaderFind((GameObject)_asset);
                }
                result = GameObject.Instantiate(_asset) as object;
            }
            catch (Exception e)
            {
                DebugX.LogError("getNewInstance Error:{0}", e.Message);
            }

            return result;
        }

        private Sprite[] _spriteList;

        public override Sprite getSpriteByName(string name)
        {
            if (_spriteList == null)
            {
                _spriteList = ((AssetBundle) _data).LoadAllAssets<Sprite>();
            }
            int len = _spriteList.Length;
            if (len > 0)
            {
                for (int i = 0; i < len; i++)
                {
                    Sprite item = _spriteList[i] as Sprite;
                    if (item.name == name)
                    {
                        return item;
                    }
                }
            }
            return null;
        }


        public static string GetPrefabNameyDependKey(string dependKey)
        {
            if (string.IsNullOrEmpty(dependKey))
            {
                return dependKey;
            }
            string[] names = dependKey.Split('/');
            string name = names[names.Length - 1];
            names = name.Split('.');
            return names[0];
        }

        public override void __dispose()
        {
            //不做除引用操作;
            if (manifestResource != null)
            {
                AssetsManager.bindEventHandle(manifestResource, manifesHandle,false);
            }
            if (dependenciesResource.Count > 0)
            {
                foreach (AssetResource assetResource in dependenciesResource)
                {
                    assetResource.removeEventListener(EventX.DEPEND_READY, dependsHandle);
                    AssetsManager.bindEventHandle(assetResource, dependsHandle, false);
                    assetResource.release();
                }
                dependenciesResource.Clear();
            }

            if (_spriteList != null)
            {
                _spriteList = null;
            }
            if (_asset != null)
            {
                GameObject.DestroyImmediate(_asset, true);
                _asset = null;
            }

            base.__dispose();
        }
    }
}
