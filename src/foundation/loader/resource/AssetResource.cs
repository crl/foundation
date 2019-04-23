using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace foundation
{
    public class AssetResource : EventDispatcher, IAutoReleaseRef
    {
        protected static Dictionary<Hash128, Hash128Link> allManifesMaping = new Dictionary<Hash128, Hash128Link>();
       
        public static bool THROW_ERROR = true;

        /// <summary>
        /// 强制超时时间(默认-1无超时策略)
        /// </summary>
        public float timeout = -1;

        /// <summary>
        /// 想要提交跟服务器的数据
        /// </summary>
        public string postData;
        /// <summary>
        /// 资源状态
        /// </summary>
        protected AssetState _status = AssetState.NONE;      
        internal LoaderXDataType parserType;
        /// <summary>
        /// 是否强制从远程加载
        /// </summary>
        public bool isForceRemote = false;

        /// <summary>
        /// 是否默认初始化主资源
        /// </summary>
        public bool isAutoMainAsset = false;
        public int maxPoolSize = 10;

        protected Stack<PoolItem> pool; 
        /// <summary>
        /// 用于定义数据(供临时使用); 
        /// </summary>
        public object userData;
        protected object _data;
        protected string _url;
        internal RFLoader loader;
        public bool isLoaded
        {
            get
            {
                return (_status == AssetState.READY || _status == AssetState.FAILD);
            }
        }

        public bool isReady
        {
            get
            {
                return _status == AssetState.READY;
            }
        }

        public AssetResource(string url)
        {
            _url = url;
        }

        public string url
        {
            get
            {
                return _url;
            }
        }

        public AssetState status
        {
            get
            {
                return _status;
            }
        }

        public LoaderXDataType getParserType()
        {
            return parserType;
        }


        public virtual void load(uint retryCount = 0, bool progress = false,int priority = 0)
        {
            if (_status == AssetState.LOADING)
            {
                //DebugX.Log("assetLoading:" + _url);
                return;
            }

            if (isLoaded)
            {
                if (_status == AssetState.FAILD)
                {
                    if (WebRequestLoader.mapping404[_url] == true)
                    {
                        this.dispatchEvent(new EventX(EventX.FAILED, "404"));
                        return;
                    }

                    //重新加载;
                    _status = AssetState.LOADING;
                    _loadImp(priority, progress, retryCount);       // 开始加载
                    return;
                }
                //Debug.Log(DateUtils.getSimple(DateTime.Now) + ":" + url + "");
                this.dispatchEvent(new EventX(EventX.COMPLETE, _data));
                return;
            }

            if (string.IsNullOrEmpty(url))
            {
                _status = AssetState.FAILD;
                DebugX.Log("assetLoading:" + _url);
                this.dispatchEvent(new EventX(EventX.FAILED, "url is empty"));
                return;
            }

            _status = AssetState.LOADING;
            _loadImp(priority, progress, retryCount);       // 开始加载
        }

        internal AssetBundleManifestDef assetBundleManifestDef;

        internal void initManifes(string manifesPrefix, string manifesKey)
        {
            assetBundleManifestDef = new AssetBundleManifestDef();
            assetBundleManifestDef.manifesKey = manifesKey;
            assetBundleManifestDef.manifesPrefix = manifesPrefix;
        }

        public static ILoaderFactory LoaderFactory;
        protected virtual void _loadImp(int priority = 0, bool progress = false, uint retryCount = 0)
        {
            if (loader == null)
            {
                if (LoaderFactory == null)
                {
                    LoaderFactory = new ResourceLoaderManager();
                }
                loader = LoaderFactory.getLoader(this);
            }
            loader.checkProgress = progress;
            loader.retryCount = retryCount;

            AssetsManager.bindEventHandle(loader,loadComplete);
            if (progress)
            {
                loader.addEventListener(EventX.PROGRESS, progressHandle);
            }
            loader.addEventListener(EventX.CANCEL, loadCancelHandle);
            AbstractApp.coreLoaderQueue.add(loader);
        }

        protected virtual void progressHandle(EventX e)
        {
            this.dispatchEvent(e);
        }

        protected virtual void loadCancelHandle(EventX e)
        {
            loader.removeEventListener(EventX.CANCEL, loadCancelHandle);
            if (_status == AssetState.LOADING)
            {
                _status=AssetState.NONE;
            }

            if (hasEventListener(EventX.FAILED))
            {
                simpleDispatch(EventX.FAILED);
            }
        }

        /**
         *  加载资源成功 
         * @param event
         * 
         */

        protected virtual void loadComplete(EventX e)
        {
            loader = e.target as RFLoader;
            AssetsManager.bindEventHandle(loader, loadComplete, false);
            loader.removeEventListener(EventX.CANCEL, loadCancelHandle);
            if (loader.checkProgress)
            {
                loader.removeEventListener(EventX.PROGRESS, progressHandle);
            }

            if (e.type == EventX.FAILED)
            {
                _data = null;
                this.simpleDispatch(EventX.DEPEND_READY);
                resourceComplete(EventX.FAILED, (string)e.data);
                return;
            }

            _data = e.data;
            if (_data == null)
            {
                _data = "_data is null";
                this.simpleDispatch(EventX.DEPEND_READY);
                resourceComplete(EventX.FAILED, (string)_data);
            }
            else
            {
                _data=parserData(_data);
                this.simpleDispatch(EventX.DEPEND_READY);
                resourceComplete(EventX.COMPLETE);
            }
        }

        protected virtual object parserData(object data)
        {
            switch (parserType)
            {
                case LoaderXDataType.AMF:

                    ByteArray bytes = new ByteArray((byte[]) data);
                    if (bytes.BytesAvailable > 0)
                    {
                        try
                        {
                            data = bytes.ReadObject();
                        }
                        catch (Exception ex)
                        {
                            data = null;
                            try
                            {
                                bytes.Inflate();
                                data = bytes.ReadObject();
                            }
                            catch (Exception)
                            {
                                bytes.Position = 0;
                                DebugX.Log("amfError:" + _url + " \tmsg:" + ex);
                            }
                        }
                    }
                    else
                    {
                        data = null;
                    }
                    break;

                case LoaderXDataType.MANIFEST:
                    AssetBundle assetBundle = (AssetBundle) data;
                    AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    data = manifest;
                    assetBundleManifestDef.manifest = manifest;

//                    string[] allAssetBundles = manifest.GetAllAssetBundles();
//                    foreach (string item in allAssetBundles)
//                    {
//                        Hash128 hash128 = manifest.GetAssetBundleHash(item);
//                        Hash128Link oldLink;
//                        if (allManifesMaping.TryGetValue(hash128, out oldLink))
//                        {
//                            UnityEngine.Debug.Log(item + " routerPath:" + oldLink.manifestDef + " new:" +
//                                                  assetBundleManifestDef);
//                        }
//                        else
//                        {
//                            Hash128Link link = new Hash128Link();
//                            link.hash128 = hash128;
//                            link.key = item;
//                            link.manifestDef = assetBundleManifestDef;
//                            allManifesMaping.Add(hash128, link);
//                        }
//                    }
                    break;
            }
            return data;
        }

        protected virtual void resourceComplete(string eventType,string errorMessage="")
        {
            if (eventType == EventX.COMPLETE)
            {
                _status = AssetState.READY;
                this.simpleDispatch(eventType, _data);
            }
            else
            {
                _status = AssetState.FAILD;
                this.simpleDispatch(eventType, errorMessage);
            }
        }

        public virtual void __dispose()
        {
            if (loader != null)
            {
                loader.removeEventListener(EventX.COMPLETE, loadComplete);
                loader.removeEventListener(EventX.PROGRESS, progressHandle);
                loader.removeEventListener(EventX.FAILED, loadComplete);
                loader.Dispose();
                loader =null;
            }

            if (pool != null)
            {
                PoolItem[] list=pool.ToArray();
                foreach (PoolItem item in list)
                {
                    item.Dispose();
                }
                pool.Clear();
                pool = null;
            }
            _status = AssetState.NONE;
            if (_data != null)
            {
                _data = null;
                //todo data;
            }

            if (_sprite != null)
            {
                GameObject.Destroy(_sprite);
                _sprite = null;
            }

            Log("AssetDispose:{0}", url);
            _disposed = true;
            this.simpleDispatch(EventX.DISPOSE);

            _clear();
        }

        private bool _disposed = false;
        public bool isDispose
        {
            get { return _disposed; }
        }

        private int _reference = 0;


        /// <summary>
        /// 加入资源引用计数; 
        /// </summary>
        /// <returns></returns>
        public int release()
        {
            if (--_reference < 1)
            {
                _reference = 0;
                AutoReleasePool.add(this);
            }
            return _reference;
        }

        public int retain()
        {
            if (++_reference == 1)
            {
                AutoReleasePool.remove(this);
            }
            return _reference;
        }
        public int retainCount
        {
            get
            {
                return _reference;
            }
        }

        public virtual object data
        {
            get { return _data; }
        }

        public virtual object getMainAsset()
        {
            return _data;
        }


        public virtual object getNewInstance()
        {
            object result = null;
            if (_data is UnityEngine.Object)
            {
                if (_data is GameObject)
                {
                    if (isShaderFinded == false)
                    {
                        isShaderFinded = true;
                        RenderUtils.ShaderFind((GameObject)_data);
                    }
                }

                try
                {
                    result = GameObject.Instantiate(_data as UnityEngine.Object) as object;
                }
                catch (Exception e)
                {
                    DebugX.LogError("getNewInstance Error:{0}", e.Message);
                }
            }
            return result;
        }

        protected bool isShaderFinded = false;

        public virtual T getPoolItemFromPool<T>() where T : PoolItem
        {
            PoolItem result = null;
            GameObject go = null;
            GameObject prefab = getMainAsset() as GameObject;
            if (prefab == null)
            {
                return default(T);
            }
            if (isShaderFinded == false)
            {
                isShaderFinded = true;
                RenderUtils.ShaderFind(prefab);
            }
            if (pool == null)
            {
                pool = new Stack<PoolItem>();
            }

            while (pool.Count > 0)
            {
                result = pool.Pop();
                if (result != null)
                {
                    result.isNew = false;
                    go = result.gameObject;
                    go.SetActive(true);
                    break;
                }
            }

            if (go == null)
            {
                go = GameObject.Instantiate<GameObject>(prefab);
                result = go.AddComponent<T>();

                if (go.activeSelf == false)
                {
                    go.SetActive(true);
                }
                result.manager = this;
            }
            return (T) result;
        }

        public virtual bool recycleToPool(PoolItem poolItem)
        {
            if (pool == null)
            {
                pool = new Stack<PoolItem>();
            }

            GameObject go = poolItem.gameObject;
            if (pool.Count < maxPoolSize)
            {
                go.SetActive(false);
                pool.Push(poolItem);
                return true;
            }
            GameObject.Destroy(go);
            return false;
        }


        public string[] getAllScenePaths()
        {
            AssetBundle assetBundle = (AssetBundle)data;
            if (assetBundle != null)
            {
                return assetBundle.GetAllScenePaths();
            }
            return null;
        }

        private Sprite _sprite;

        public Sprite getSprite()
        {
            if (_sprite != null)
            {
                return _sprite;
            }

            Texture2D texture = _data as Texture2D;
            if (texture != null)
            {
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Vector2 p = new Vector2();
                _sprite = Sprite.Create(texture, rect, p,100,0,SpriteMeshType.FullRect);
            }

            return _sprite;
        }

        public Texture getTexture()
        {
            return _data as Texture;
        }


        public XmlDocument getXML()
        {
            XmlDocument doc = new XmlDocument();
            MemoryStream stream = new MemoryStream((byte[])_data);
            try
            {
                doc.Load(stream);
            }
            catch (Exception e)
            {
                DebugX.Log("parserXML error:" + e);
            }

            return doc;
        }

        public virtual Sprite getSpriteByName(string name)
        {
			object[] list = (object[]) _data;
            for (int i = 0; i < list.Length; i++)
            {
                Sprite item = list[i] as Sprite;
                if (item.name == name)
                {
                    return item;
					//break;
                }
            }
            return null;
        }

        public static bool DEBUG = false;
        protected static void Log(string v, params object[] parms)
        {
            if (DEBUG)
            {
                string message = StringUtil.substitute(v, parms);
                DebugX.Log(message);
            }
        }

    }
}
