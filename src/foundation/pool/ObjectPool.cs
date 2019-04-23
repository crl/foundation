using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class ObjectPool<T> where T:new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public ObjectPool()
        {
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
                if (element is IPoolable)
                {
                    ((IPoolable)element).poolAwake();
                }
            }
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");

            if (element is IPoolable)
            {
                ((IPoolable) element).poolRecycle();
            }
            m_Stack.Push(element);
        }
    }
}
