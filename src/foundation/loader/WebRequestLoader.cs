using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace foundation
{
    internal class WebRequestLoader : RFLoader
    {
        private Coroutine coroutine;
        public WebRequestLoader(string url, LoaderXDataType parserType) : base(url, parserType)
        {
        }

        public override void load()
        {
            if (string.IsNullOrEmpty(_url))
            {
                this.dispatchEvent(new EventX(EventX.FAILED, "文件路径为空:" + _url));
                return;
            }

            if (_data != null)
            {
                this.simpleDispatch(EventX.COMPLETE, _data);
                return;
            }

            if (_status == LoadState.LOADING)
            {
                //正在加载中;
                DebugX.LogWarning("正在load资源:" + _url);
                return;
            }

            if (!isLocalFile)
            {
                DebugX.LogWarning("从远程加载资源:" + _url);
            }
            _retryedCount = 0;

            _status = LoadState.LOADING;
            if (coroutine!=null)
            {
                StopCoroutine(coroutine);
            }
            coroutine=StartCoroutine(doLoad(_url));
        }

        protected void retryLoad()
        {
            DebugX.LogWarning("{0}重试:{1}", _retryedCount, _url);
            _status = LoadState.LOADING;

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine=StartCoroutine(doLoad(_url));
        }

        private IEnumerator doLoad(string url)
        {
            UnityWebRequest request;
            switch (_parserType)
            {
                case LoaderXDataType.BYTES:
                case LoaderXDataType.AMF:
                case LoaderXDataType.TEXTURE:
                    request = UnityWebRequest.Get(url);
                    break;
                case LoaderXDataType.MANIFEST:
                case LoaderXDataType.ASSETBUNDLE:
                    request =UnityWebRequest.Get(url);
                    break;
                case LoaderXDataType.POST:
                    request = UnityWebRequest.Post(url, postData);
                    break;
                case LoaderXDataType.GET:
                    string fullPath = url;
                    if (string.IsNullOrEmpty(postData)==false)
                    {
                        fullPath = url + "?" + postData;
                    }
                    request = UnityWebRequest.Get(fullPath);
                    break;
                default:
                    request = UnityWebRequest.Get(url);
                    break;
            }

            float stratTime = Time.realtimeSinceStartup;
            bool isTimeout = false;
            if (timeout > 0)
            {
                request.timeout = Mathf.CeilToInt(timeout);
            }
            while (!request.isDone)
            {
                if (timeout > 0 && (Time.realtimeSinceStartup - stratTime) > (timeout * 1.2f))
                {
                    isTimeout = true;
                    break;
                }

                if (checkProgress)
                {
                    update(request.downloadProgress);
                }

                yield return request.Send();
            }

            long responseCode = request.responseCode;
            if (request.isError || (responseCode!=200 &&responseCode!=204) || isTimeout)
            {
                string error="code=" + responseCode;
                if (isTimeout)
                {
                    error += ",error=isTimeout:" + timeout;
                }
                else if (request.isError)
                {
                    error += ",error=" + request.error;
                }
                else
                {
                    if (responseCode == 404)
                    {
                        mapping404[_url] = true;
                    }
                }
                _status = LoadState.ERROR;
                string message = string.Format("下载文件失败:{0} reason:{1}", _url, error);
                DebugX.LogWarning(message);

                request.Dispose();
                request = null;

                if (retryCount > _retryedCount)
                {
                    _retryedCount++;
                    _status = LoadState.LOADING;
                    ///本身加载需要时间,所以不必later太长
                    CallLater.Add(retryLoad, 1.0f);
                    yield break; 
                }

                this.simpleDispatch(EventX.FAILED, message);
            }
            else
            {
                _status = LoadState.COMPLETE;
                switch (_parserType)
                {
                    case LoaderXDataType.BYTES:
                    case LoaderXDataType.AMF:
                        _data = request.downloadHandler.data;
                        break;
                    case LoaderXDataType.ASSETBUNDLE:
                    case LoaderXDataType.MANIFEST:
                        onAssetBundleHandle(AssetBundle.LoadFromMemory(request.downloadHandler.data));
                        break;
                    case LoaderXDataType.TEXTURE:
                        //linner;
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false, false);
                        tex.LoadImage(request.downloadHandler.data);
                        _data = tex;
                        break;
                    default:
                        _data = request.downloadHandler.data;
                        break;
                }
                request.Dispose();
                request = null;
                this.simpleDispatch(EventX.COMPLETE, _data);
            }
            selfComplete();
        }

        protected override void clearData()
        {
            switch (_parserType)
            {
                case LoaderXDataType.TEXTURE:
                    UnityEngine.Object o = _data as UnityEngine.Object;
                    if (o != null)
                    {
                        GameObject.DestroyImmediate(o, true);
                    }
                   
                    break;
            }
            _status = LoadState.NONE;
            _data = null;
        }
    }
}

