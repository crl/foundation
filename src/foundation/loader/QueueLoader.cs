using System;
using System.Collections.Generic;

namespace foundation
{
    public class QueueLoader : EventDispatcher
    {
        public static bool DEBUG = false;
        private static Stack<QueueLoader> queueLoaderPool = new Stack<QueueLoader>();
        private Queue<AssetResource> queue = new Queue<AssetResource>();
        private Dictionary<string, Action<EventX>> resultActions = new Dictionary<string, Action<EventX>>();
        private HashSet<AssetResource> runningList = new HashSet<AssetResource>();
        private Dictionary<string, AssetResource> urlMapping = new Dictionary<string, AssetResource>();
        public uint retryCount = 0;
        //是否内部使用引用计数
        public bool isUseRef = false;
        public int maxFailCount = -1;
        private int threadCount = 1;

        private int total = 0;
        private int loaded = 0;
        private int running = 0;

        private bool isStart = false;
        private bool isComplete = false;
        public static QueueLoader Get()
        {
            if (queueLoaderPool.Count > 0)
            {
                return queueLoaderPool.Pop();
            }
            return new QueueLoader();
        }

        public static void Recycle(QueueLoader value)
        {
            if (value == null)
            {
                return;
            }
            value.recycle();
            if (queueLoaderPool.Count < 100)
            {
                queueLoaderPool.Push(value);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="uri"></param>
        /// <param name="type"></param>
        /// <param name="resultHandle">同url只返回一次回调</param>
        /// <returns></returns>
        public AssetResource add(string url, LoaderXDataType type, Action<EventX> resultHandle = null)
        {
            if (isStart)
            {
                DebugX.LogError("Qu不能在运行时加入:"+url);
            }

            AssetResource resource = null;
            
            if (string.IsNullOrEmpty(url))
            {
                return resource;
            }

            string key = url.ToLower();
            if (urlMapping.TryGetValue(key, out resource))
            {
                return resource;
            }
            resource = AssetsManager.getResource(url, type);
            if (resource == null)
            {
                return null;
            }

            if (isUseRef)
            {
                resource.retain();
            }

            urlMapping.Add(key, resource);
            queue.Enqueue(resource);
            if (resultHandle != null && resultActions.ContainsKey(key) == false)
            {
                resultActions.Add(key, resultHandle);
            }

            return resource;
        }

        public void start(int threadCount = -1)
        {
            if (threadCount == -1)
            {
                threadCount = Math.Max(CoreLoaderQueue.CONCURRENCE, 2);
            }
            isStart = true;
            isComplete = false;
            this.total = queue.Count;

            this.threadCount = threadCount;
            this.running = 0;
            this.loaded = 0;
            _doNext();
        }

        public int length
        {
            get { return queue.Count; }
        }

        private void _doNext()
        {
            if (isComplete)
            {
                return;
            }
            testCount++;
            if (queue.Count == 0 && running == 0 && isComplete==false)
            {
                isComplete =true;
                Log("QuComplete");
                this.simpleDispatch(EventX.COMPLETE);
                return;
            }
           
            while (queue.Count > 0 && running < threadCount)
            {
                AssetResource resource = queue.Dequeue();
                runningList.Add(resource);

                running++;
                Log("QuStart: {0} {1}", running, resource.url);

                AssetsManager.bindEventHandle(resource, itemComplete);
                resource.addEventListener(EventX.DISPOSE, itemComplete);
                resource.addEventListener(EventX.PROGRESS, itemProgress);
                resource.load(retryCount,false);

            }
        }

        private float preProgress = 0.0f;
        private void itemProgress(EventX e)
        {
            float v = 1.0f/total*((float)e.data);
            if (v > preProgress)
            {
                preProgress = v;
                this.simpleDispatch(EventX.PROGRESS, (loaded + v)/(float) total);
            }
        }

        private int faildCount = 0;

        private int testCount=0;
        private void itemComplete(EventX e)
        {
            AssetResource resource = e.target as AssetResource;
            AssetsManager.bindEventHandle(resource, itemComplete, false);
            resource.removeEventListener(EventX.PROGRESS, itemProgress);
            resource.removeEventListener(EventX.DISPOSE, itemComplete);

            runningList.Remove(resource);
            running--;
            Log("QuEnd: {0} {1},{2},{3}", running, e.type, resource.url, queue.Count);

            if (running < 0)
            {
                DebugX.LogError("queueLoader error runnig=" + running);
            }

            if (e.type != EventX.COMPLETE)
            {
                faildCount++;
                if (maxFailCount > 0 && faildCount > maxFailCount)
                {
                    this.simpleDispatch(EventX.FAILED, e);
                }
                DebugX.LogWarning("QuItemFailed:" + resource.url + ":" + e.type+" faildCount:"+ faildCount+"/"+ maxFailCount);
            }

            preProgress = 0.0f;
            loaded = total - (queue.Count + running);
            //DebugX.Log("kk:" + (loaded) / (float)total);
            this.simpleDispatch(EventX.PROGRESS, loaded/(float) total);

            Action<EventX> action = null;

            string key = resource.url.ToLower();
            if (resultActions.TryGetValue(key, out action))
            {
                action(e);
            }

            _doNext();
        }

        public int getFaildCount()
        {
            return faildCount;
        }

        public int getLoaded()
        {
            return loaded;
        }

        public int getTotal()
        {
            return total;
        }

        public Dictionary<string, AssetResource> getMaping()
        {
            return urlMapping;
        }

        public void recycle()
        {
            if (runningList.Count > 0)
            {
                foreach (AssetResource resource in runningList)
                {
                    if (isUseRef)
                    {
                        resource.release();
                    }
                    AssetsManager.bindEventHandle(resource, itemComplete,false);
                    resource.removeEventListener(EventX.PROGRESS, itemProgress);
                    resource.removeEventListener(EventX.DISPOSE, itemComplete);
                }
                runningList.Clear();
            }

            urlMapping.Clear();
            queue.Clear();
            resultActions.Clear();

            running = 0;
            total = 0;
            loaded = 0;
            _clear();
            isComplete = false;
            isStart = false;
        }


        private static void Log(string v, params object[] parms)
        {
            if (DEBUG)
            {
                string message = StringUtil.substitute(v, parms);
                DebugX.Log(message);
            }
        }

    }
}
