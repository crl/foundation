using foundation;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace gameSDK
{
    /// <summary>
    /// 项目配置管理
    /// </summary>
    public class ProjectConfigVODB:EventDispatcher
    {
        public string extention = "jat";
        protected QueueLoader queueLoader;
        protected List<IConfigVODB> dbs;
        protected FrameTask<MemoryStream> frameTask;
        public ProjectConfigVODB()
        {
            queueLoader=new QueueLoader();
            queueLoader.addEventListener(EventX.PROGRESS, progressHandle);
            queueLoader.addEventListener(EventX.COMPLETE, completeHandle);

            frameTask = new FrameTask<MemoryStream>();
            frameTask.addEventListener(EventX.COMPLETE, parserCompleteHandle);
            frameTask.addEventListener(EventX.PROGRESS, progressHandle);

            dbs=new List<IConfigVODB>();
        }

        protected virtual void AddDB(IConfigVODB value,bool isForceRemote)
        {
            string uri = value.GetFileName();
            if (string.IsNullOrEmpty(uri) == false)
            {
                string url = PathDefine.configPath + uri + "." + extention;
                AssetResource resource = queueLoader.add(url, LoaderXDataType.BYTES, itemComplete);
                resource.userData = value;
                resource.isForceRemote = isForceRemote;
            }
            dbs.Add(value);
        }

        public QueueLoader GetQueueLoader()
        {
            return queueLoader;
        }

        public void StartLoad(int threadCount = -1)
        {
            queueLoader.start(threadCount);
        }

        public void StartParser()
        {
            frameTask.start();
        }

        protected virtual void itemComplete(EventX e)
        {
            AssetResource resource = e.target as AssetResource;
            if (e.type != EventX.COMPLETE)
            {
                this.simpleDispatch(EventX.ERROR,"有加载文件失败:" + e.data);
                return;
            }

            IConfigVODB danager = resource.userData as IConfigVODB;
            if (danager != null)
            {
                byte[] buffer = e.data as byte[];
                MemoryStream _memoryStream = new MemoryStream();
                _memoryStream.Write(buffer, 0, buffer.Length);
                _memoryStream.Position = 0;

                addParseItem(danager, _memoryStream);
            }
        }

        public void addParseItem(IConfigVODB danager, MemoryStream bytes)
        {
            frameTask.addItem(danager.Deserialize, bytes);
        }

        protected virtual void progressHandle(EventX e)
        {
            this.simpleDispatch(EventX.PROGRESS,e.data);
        }
        protected virtual void completeHandle(EventX e)
        {
            queueLoader.removeEventListener(EventX.COMPLETE, completeHandle);
            queueLoader.removeEventListener(EventX.PROGRESS, progressHandle);

            queueLoader.recycle();

            StartParser();
        }

        protected virtual void parserCompleteHandle(EventX e)
        {
            frameTask.removeEventListener(EventX.COMPLETE, parserCompleteHandle);
            frameTask.removeEventListener(EventX.PROGRESS, progressHandle);

            foreach (IConfigVODB db in dbs)
            {
                db.Initialization();
            }
            this.simpleDispatch(EventX.COMPLETE);
        }
    }
}