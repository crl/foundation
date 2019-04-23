using System;
using System.Collections;
using System.Collections.Generic;

namespace foundation
{
    /// <summary>
    ///  像as一样操作的Array(默认实现);
    /// </summary>
    public class ASList : ASList<object>
    {
        
    }
    /// <summary>
    /// 像as一样操作的Array;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ASList<T>:IEnumerable<T>
    {
        private List<T> list;

        public ASList(int capacity)
        {
            list=new List<T>(capacity);
        }

        public List<T> GetUnsafeList()
        {
            return list;
        }

        public ASList()
        {
            list = new List<T>();
        }

        public void Add(T value)
        {
            list.Add(value);
        }

        public void Adds(params T[] args)
        {
            foreach (T item in args)
            {
                list.Add(item);
            }
        }

        public void AddRange(T[] value)
        {
            foreach (T item in value)
            {
                list.Add(item);
            }
        }

        public int IndexOf(T value)
        {
            return list.IndexOf(value);
        }

        public T Pop()
        {
            int index = list.Count;
            if (index == 0)
            {
                return default(T);
            }

            index -= 1;
            T value=list[index];

            list.RemoveAt(index);

            return value;
        }

        public T Shift()
        {
            int index = list.Count;
            if (index == 0)
            {
                return default(T);
            }
            T value = list[0];
            list.RemoveAt(0);

            return value;
        }

        public void Insert(int index, T value)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (index >= list.Count)
            {
                list.Add(value);
                return;
            }

            list.Insert(index,value);
        }

        public void Clear()
        {
            list.Clear();
        }

        public int Count
        {
            get { return list.Count; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (value == 0)
                {
                    list.Clear();
                    return;
                }

                int dis = list.Count - value;
                if (dis > 0)
                {
                    while (dis-- > 0)
                    {
                        list.RemoveAt(list.Count - 1);
                    }
                }
                else if (dis < 0)
                {
                    dis = Math.Abs(dis);

                    while (dis-- > 0)
                    {
                        list.Add(default(T));
                    }
                }
            }
        }

        public bool Contains(T value)
        {
            return list.Contains(value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                if (index > list.Count-1 || index<0)
                {
                    return default(T);
                }
                return list[index];
            }

            set
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException("不能小于0");
                }

                if (index>list.Count-1)
                {
                    for (int i = list.Count; i < index+1; i++)
                    {
                        list.Add(default(T));
                    }
                }
                list[index] = value;
            }
        }

        public bool RemoveAt(int index)
        {
            if (index<0 || index > list.Count)
            {
                return false;
            }
            list.RemoveAt(index);
            return true;
        }
    }
}