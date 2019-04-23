using System.Collections.Generic;

namespace foundation
{
    public class CoreLoaderQueue:FoundationBehaviour
    {
        public static bool DEBUG = false;
        public static int MAX = 300;
        /// <summary>
        /// 默认限制数量
        /// </summary>
        public static int CONCURRENCE = 4;

        protected Queue<RFLoader> queue=new Queue<RFLoader>();
        protected List<RFLoader> runingList=new List<RFLoader>(); 

        private int running = 0;

        public bool add(RFLoader loader)
        {
            if (runingList.Contains(loader) || queue.Contains(loader))
            {
                return false;
            }
            queue.Enqueue(loader);
            return true;
        }

        private void Update()
        {
            if (running < CONCURRENCE && queue.Count > 0)
            {
                RFLoader loader = queue.Dequeue();
                loader.addEventListener(EventX.COMPLETE, loadHandle, MAX);
                loader.addEventListener(EventX.FAILED, loadHandle, MAX);
                loader.addEventListener(EventX.DISPOSE, loadHandle, MAX);

                runingList.Add(loader);
                running++;
                Log("CorStart: {0} {1} {2}", running, loader._url, loader.ToString());

                loader.load();
            }
        }

        private static void Log(string v, params object[] parms)
        {
            if (DEBUG)
            {
                string message = StringUtil.substitute(v, parms);
                DebugX.Log(message);
            }
        }

        private void loadHandle(EventX e)
        {
            RFLoader target = (RFLoader)e.target;
            runingList.Remove(target);
            running--;

            Log("CorEnd: {0} {1},{2}", running, e.type, target._url);

            if (running < 0)
            {
                DebugX.LogError("coreLoaderQueue error {0} {1},{2}",running, e.type, target._url);
            }

            target.removeEventListener(EventX.COMPLETE, loadHandle);
            target.removeEventListener(EventX.FAILED, loadHandle);
            target.removeEventListener(EventX.DISPOSE, loadHandle);
        }
    }

}
