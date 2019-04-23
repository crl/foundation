using System;
using System.Collections.Generic;

namespace foundation
{
    public class QueueHandle<T>
    {
        private static Stack<SignalNode<T>> NodePool=new Stack<SignalNode<T>>();
		private static int MAX=1000;
		
		internal SignalNode<T> firstNode;
		internal SignalNode<T> lastNode;
		
		protected Dictionary<Action<T>, SignalNode<T>> maping;
		
		protected int len =0;
		
		internal bool dispatching=false;

        public QueueHandle()
        {
        }


        public int length{
            get
            {
                return len;
            }
		}

        private static Stack<List<SignalNode<T>>> SignalNodeListPool = new Stack<List<SignalNode<T>>>();
        protected static List<SignalNode<T>> GetSignalNodeList()
        {
            if (SignalNodeListPool.Count>0)
            {
                List<SignalNode<T>> temp= SignalNodeListPool.Pop();
                temp.Clear();
                return temp;
            }

            return new List<SignalNode<T>>();
        }

        protected static void Recycle(List<SignalNode<T>> node)
        {
            if (SignalNodeListPool.Count <300)
            {
                SignalNodeListPool.Push(node);
            }
        }

        public void dispatch(T e)
        {
            if (len > 0)
            {
                dispatching = true;
                SignalNode<T> t = firstNode;

                List<SignalNode<T>> temp=GetSignalNodeList();

                while (t != null)
                {
                    if (t.active == NodeActiveState.Runing)
                    {
                        t.action(e);
                    }
                    temp.Add(t);
                     t = t.next;
                }
                dispatching = false;

                int l = temp.Count;
                for (int i = 0; i < l; i++)
                {
                    SignalNode<T> item = temp[i];
                
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



        public bool ___addHandle(Action<T> value, T data, bool forceData = false)
        {
            if (maping == null) {
                maping = new Dictionary<Action<T>, SignalNode<T>>();
            }
            SignalNode<T> t =null;
            if (maping.TryGetValue(value, out t))
            {
                if (t.active == NodeActiveState.ToDoDelete)
                {
                    if (dispatching)
                    {
                        t.active = NodeActiveState.ToDoAdd;
                    }
                    else
                    {
                        t.active = NodeActiveState.Runing;
                    }
                    t.data = data;
                    return true;
                }
                if (forceData)
                {
                    t.data = data;
                }
                return false;
            }

            t = getSignalNode();
            t.action = value;
            t.data = data;
            maping.Add(value, t);

            if (dispatching) {
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

        protected SignalNode<T> getSignalNode()
        {
            SignalNode<T> t;
            if (NodePool.Count > 0)
            {
                t = NodePool.Pop();
                t.active = NodeActiveState.Runing;
            }
            else
            {
                t = new SignalNode<T>();
            }
            return t;
        }


        public bool ___removeHandle(Action<T> value)
		{
			if(lastNode==null || maping==null){
				return false;
			}

            SignalNode<T> t = null;
			if(maping.TryGetValue(value,out t)==false || t.active== NodeActiveState.ToDoDelete)
            {
				return false;
			}
			
			if(dispatching){
				t.active=NodeActiveState.ToDoDelete;
				return true;
			}

            return _remove(t, value);
        }

        public bool hasHandle(Action<T> value) {
            if (maping==null)
            {
                return false;
            }
            return maping[value] != null;
        }
		
		protected bool _remove(SignalNode<T> t, Action<T> value){
		    if (t == null)
		    {
		        DebugX.LogError("queueHandle error nil");
		    }
          
            SignalNode<T> pre =t.pre;
            SignalNode<T> next =t.next;
			if(pre !=null){
				pre.next=next;
			}else{    
                firstNode =next;
			}

            if (next !=null){
				next.pre=pre;
			}else{
				lastNode=pre;
			}
			t.active= NodeActiveState.ToDoDelete;
		    
            maping.Remove(value);

            if (NodePool .Count< MAX){
				t.action=null;
                t.pre = t.next = null;
				NodePool.Push(t);
			}
			len--;

            if (len < 0)
            {
                DebugX.LogError("QueueHandle lenError:" + len);
            }

            return true;
		}
		
		
		public void _clear()
		{		
			if(null==firstNode){				
				return;
			}

            SignalNode<T> t =firstNode;
		    SignalNode<T> n;

            while (t !=null){
				t.action=null;
                if (NodePool.Count > MAX)
                {
                    break;
                }
                n = t.next;

                t.action = null;
                t.pre = t.next = null;
                NodePool.Push(t);

                t = n;
            }
			
			maping=null;
			firstNode=lastNode=null;
			len=0;
		}

    }
}
