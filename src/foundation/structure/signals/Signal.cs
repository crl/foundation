using System;
using System.Collections.Generic;

namespace foundation
{
    public class Signal:QueueHandle<EventX>
    {
        public Signal()
        {

        }

        public bool add(Action<EventX> value, int priority = 0)
        {
            if (maping == null)
            {
                maping = new Dictionary<Action<EventX>, SignalNode<EventX>>();
            }

            SignalNode<EventX> t = null;
            if (maping.TryGetValue(value,out t))
            {
                //如果已被删除过程中又被添加(好神奇的逻辑,但必然会有这种情况，不是可能);
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
                    return true;
                }
                return false;
            }

            SignalNode<EventX> newNode = getSignalNode();

            newNode.action = value;
            newNode.priority = priority;

            maping[value] = newNode;

            if (dispatching)
            {
                newNode.active = NodeActiveState.ToDoAdd;
            }

            if (firstNode == null)
            {
                len = 1;
                lastNode = firstNode = newNode;
                return true;
            }

            SignalNode<EventX> findNode = null;
            if (priority > lastNode.priority)
            {
                t = firstNode;
                SignalNode<EventX> pre;
                //var next:SignalNode;
                while (null != t)
                {
                    if (priority > t.priority)
                    {
                        pre = t.pre;
                        //next=t.next;
                        newNode.next = t;
                        t.pre = newNode;

                        if (null != pre)
                        {
                            pre.next = newNode;
                            newNode.pre = pre;
                        }
                        else
                        {
                            firstNode = newNode;
                        }

                        findNode = t;

                        break;
                    }
                    t = t.next;
                }
            }

            if (null == findNode)
            {
                lastNode.next = newNode;
                newNode.pre = lastNode;
                lastNode = newNode;
            }
            len++;
            return true;
        }
		
		
		public bool remove(Action<EventX> value){
			return ___removeHandle(value);
		}
    }
}
