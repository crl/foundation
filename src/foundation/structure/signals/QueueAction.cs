using System;
using System.Collections.Generic;

namespace foundation
{
    public class QueueAction<T>
    {
        private static Stack<ActionNode<T>> NodePool = new Stack<ActionNode<T>>();
        private static int MAX = 1000;

        internal ActionNode<T> firstNode;
        internal ActionNode<T> lastNode;

        protected Dictionary<Action, ActionNode<T>> maping;

        protected int len = 0;

        internal bool dispatching = false;

        public QueueAction()
        {
        }


        public int length
        {
            get
            {
                return len;
            }
        }

        protected static Stack<List<ActionNode<T>>> SignalNodeListPool = new Stack<List<ActionNode<T>>>();
        protected static List<ActionNode<T>> GetSignalNodeList()
        {
            if (SignalNodeListPool.Count > 0)
            {
                List<ActionNode<T>> temp = SignalNodeListPool.Pop();
                temp.Clear();
                return temp;
            }

            return new List<ActionNode<T>>();
        }

        protected static void Recycle(List<ActionNode<T>> node)
        {
            if (SignalNodeListPool.Count < 300)
            {
                SignalNodeListPool.Push(node);
            }
        }

        public void dispatch()
        {
            if (len > 0)
            {
                dispatching = true;
                ActionNode<T> t = firstNode;

                List<ActionNode<T>> temp = GetSignalNodeList();

                while (t != null)
                {
                    if (t.active == NodeActiveState.Runing)
                    {
                        t.action();
                    }
                    temp.Add(t);
                    t = t.next;
                }
                dispatching = false;

                int l = temp.Count;
                for (int i = 0; i < l; i++)
                {
                    ActionNode<T> item = temp[i];
                    if (item.active == NodeActiveState.ToDoDelete)
                    {
                        _remove(item, item.action);
                    }
                    else if (item.active == NodeActiveState.ToDoAdd)
                    {
                        item.active = NodeActiveState.Runing;
                    }
                }
                Recycle(temp);
            }
        }



        public bool ___addHandle(Action value,T data,bool forceData=false)
        {
            if (maping == null)
            {
                maping = new Dictionary<Action, ActionNode<T>>();
            }
            ActionNode<T> t = null;
            if (maping.TryGetValue(value, out t))
            {
                if (t.active == NodeActiveState.ToDoDelete)
                {
                    if (dispatching)
                    {
                        t.active = NodeActiveState.ToDoAdd;
                    }
                    else {
                        t.active = NodeActiveState.Runing;
                    }
                    t.data = data;
                    return true;
                }

                if (forceData)
                {
                    t.data = data;
                }

                //DebugX.Log(this.GetType().FullName + ":addReplace" + data.ToString());
                return false;
            }

            //DebugX.Log(this.GetType().FullName+":add"+data.ToString());

            t = getSignalNode();
            t.action = value;
            t.data = data;
            maping.Add(value, t);

            if (dispatching)
            {
                t.active = NodeActiveState.ToDoAdd;
            }

            if (lastNode != null)
            {
                lastNode.next = t;
                t.pre = lastNode;
                lastNode = t;
            }
            else
            {
                firstNode = lastNode = t;
            }

            len++;

            return true;
        }

        protected ActionNode<T> getSignalNode()
        {
            ActionNode<T> t;
            if (NodePool.Count > 0)
            {
                t = NodePool.Pop();
                t.active = NodeActiveState.Runing;
            }
            else
            {
                t = new ActionNode<T>();
            }
            return t;
        }


        public bool ___removeHandle(Action value)
        {
            if (lastNode == null || maping == null)
            {
                return false;
            }
    
            ActionNode<T> t = null;
            if (maping.TryGetValue(value, out t) == false || t.active == NodeActiveState.ToDoDelete)
            {
                return false;
            }

            if (dispatching)
            {
                t.active = NodeActiveState.ToDoDelete;
                return true;
            }

            return _remove(t, value);
        }

        public bool hasHandle(Action value)
        {
            if (maping == null)
            {
                return false;
            }

            return maping.ContainsKey(value);
        }

        protected bool _remove(ActionNode<T> t, Action value)
        {
            if (t == null)
            {
                DebugX.LogError("queueAction error nil");
            }

            ActionNode<T> pre = t.pre;
            ActionNode<T> next = t.next;
            if (pre != null)
            {
                pre.next = next;
            }
            else {
                firstNode = next;
            }
            
            if (next != null)
            {
                next.pre = pre;
            }
            else {
                lastNode = pre;
            }
            t.active = NodeActiveState.ToDoDelete;

            maping.Remove(value);

            if (NodePool.Count < MAX)
            {
                t.action = null;
                t.pre = t.next = null;
                NodePool.Push(t);
            }
            len--;


            if (len < 0)
            {
                DebugX.LogError("QueueAction lenError:"+len);
            }

            return true;
        }


        public void _clear()
        {
            if (null == firstNode)
            {
                return;
            }

            ActionNode<T> t = firstNode;
            ActionNode<T> n;
            while (t != null)
            {
                if (NodePool.Count > MAX)
                {
                    break;
                }
                n = t.next;

                t.action = null;
                t.pre = t.next  = null;
                NodePool.Push(t);
               
                t = n;
            }

            maping = null;
            firstNode = lastNode = null;
            len = 0;
        }

    }
}
