using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public abstract class RFLoader : EventDispatcher
    {
        public static float DEBUG_TIMEOUT = 5.0f;
        /// <summary>
        /// 最大超时时间();
        /// </summary>
        public float timeout = -1;

        /// <summary>
        /// 想要提交跟服务器的数据
        /// </summary>
        public string postData;
        internal bool isLocalFile = true;
        protected int _retryedCount=0;

        public static ASDictionary<string, bool> mapping404 = new ASDictionary<string, bool>();
        public static Dictionary<string, AssetBundle> assetBundleMapping = new Dictionary<string, AssetBundle>();
        public static void ADDTOMAPPING(string _url,AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                DebugX.LogWarning("assetBundle is null :"+_url);
                return;
            }
            AssetBundle old = null;
            string key = _url.ToLower();
            assetBundleMapping.TryGetValue(key, out old);
            if (old!=null)
            {
                throw new Exception("assetBundle exit:"+key);
            }

            assetBundleMapping[key] =assetBundle;
        }

        public static void REMOVEFROMMAPPING(string _url)
        {
            AssetBundle assetBundle = null;
            string key = _url.ToLower();
            if (assetBundleMapping.TryGetValue(key, out assetBundle))
            {
                assetBundleMapping.Remove(key);

                if (assetBundle != null)
                {
                    assetBundle.Unload(false);
                }
            }
        }

        public string _url;
        public bool checkProgress = false;
        public uint retryCount = 0;

        protected object _data;
        protected LoadState _status;
        protected LoaderXDataType _parserType;
        public RFLoader(string url, LoaderXDataType parserType)
        {
            this._url = url;
            this._parserType = parserType;
        }
        public virtual void load()
        {
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return AbstractApp.coreLoaderQueue.StartCoroutine(routine);
        }
        public void StopCoroutine(Coroutine coroutine)
        {
            AbstractApp.coreLoaderQueue.StopCoroutine(coroutine);
        }

        protected virtual void clearData()
        {
        }

        private float startTime = 0.0f;
        private float preTime = 0;
        private float preProgress = 0;
        private float sinceTime = 0;
        private float lastDebugTime = 0;
        protected void update(float progress)
        {
            if (startTime == 0)
            {
                startTime= lastDebugTime = Time.realtimeSinceStartup;
            }
            sinceTime = Time.realtimeSinceStartup - startTime;
            if (progress != preProgress && progress>0)
            {
                preTime = Time.realtimeSinceStartup;
                preProgress = progress;
                this.simpleDispatch(EventX.PROGRESS, preProgress);
            }
           
            if ((Time.realtimeSinceStartup - lastDebugTime) > DEBUG_TIMEOUT)
            {
                lastDebugTime= Time.realtimeSinceStartup;
                DebugX.Log(_url + " time:" + sinceTime + " pro:" + progress);
            }
        }

        protected virtual void onAssetBundleHandle(AssetBundle assetBundle)
        {
            if (assetBundle != null)
            {
                ADDTOMAPPING(_url, assetBundle);

                _status = LoadState.COMPLETE;
                _data = assetBundle;
                this.simpleDispatch(EventX.COMPLETE, _data);
            }
            else
            {
                _status = LoadState.ERROR;
                _data = null;

                string message = string.Format("load:加载:{0},数据没有assetBundle", _url);
                DebugX.LogWarning(message);
                this.simpleDispatch(EventX.FAILED, message);
            }
        }
        public LoadState status
        {
            get
            {
                return _status;
            }
        }

        protected void selfComplete()
        {
            this.simpleDispatch(EventX.COMPLETE);
        }

        public override void Dispose()
        {

            if (_status == LoadState.COMPLETE)
            {
                _status = LoadState.NONE;
                REMOVEFROMMAPPING(_url);
                clearData();
            }

            this.simpleDispatch(EventX.DISPOSE);
            //clearEvent;
            _clear();
        }
    }
}
