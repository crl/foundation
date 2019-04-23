using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class FrameTask<T>:EventDispatcher
    {
        private int parserCount = 0;
        private float timeLimit = 0.5f;

        private Queue<Func<T, Action, IEnumerator>> actionList=new Queue<Func<T, Action, IEnumerator>>();
        private Queue<T> actionDatas = new Queue<T>();
        
        public void start(float timeLimit = 0.5f)
        {
            this.timeLimit = timeLimit;

            parserCount = actionList.Count;

            doNext();
        }

        public void addItem(Func<T, Action, IEnumerator> action,T data)
        {
            actionList.Enqueue(action);
            actionDatas.Enqueue(data);
        }

        private void doNext()
        {
            if (actionList.Count==0)
            {
                this.simpleDispatch(EventX.COMPLETE);
                return;
            }

            Func<T, Action, IEnumerator> action = actionList.Dequeue();
            T data = actionDatas.Dequeue();

            this.simpleDispatch(EventX.PROGRESS, (parserCount - actionList.Count) / (float)parserCount);
            AbstractApp.coreLoaderQueue.StartCoroutine(action(data, doNext));
        }
    }
}