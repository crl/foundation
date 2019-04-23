using System.Collections;
using System.Collections.Generic;

namespace foundation
{
    public class ASDictionarySet<K,V>: IEnumerable<K>
    {
        private Dictionary<K, V> dic = new Dictionary<K, V>();
        private List<V> values=new List<V>();
        private List<K> keys = new List<K>();


        /// <summary>
        /// 个数;
        /// </summary>
        public int Count
        {
            get { return dic.Count; }
        }

        public bool TryGetValue(K key, out V value)
        {
            if (key == null)
            {
                value = default(V);
                return false;
            }
            return dic.TryGetValue(key, out value);
        }

        public void Clear()
        {
            dic.Clear();
            values.Clear();
            keys.Clear();
        }

        public V this[K key]
        {
            get
            {
                if (null == key)
                {
                    return default(V);
                }

                V data;
                dic.TryGetValue(key, out data);

                return data;
            }
            set
            {
                if (null == key)
                {
                    return;
                }
                Add(key,value);
            }
        }

        public void Add(K key, V value)
        {
            if (key == null)
            {
                return;
            }
            this.Remove(key);

            if (value == null)
            {
                return;
            }

            keys.Add(key);
            values.Add(value);
            dic.Add(key, value);
        }


        public void Remove(K key)
        {
            V value = default(V);
            if (dic.TryGetValue(key, out value) == false)
            {
                return;
            }

            dic.Remove(key);
            values.Remove(value);
            keys.Remove(key);
        }

        public bool ContainsKey(K value)
        {
            return dic.ContainsKey(value);
        }

        public bool ContainsValue(V value)
        {
            return dic.ContainsValue(value);
        }

        public List<K> UnsafeKeys
        {
            get { return keys; }
        }


        public List<V> UnsafeValues
        {
            get { return values; }
        }

        public Dictionary<K, V> UnsafeDic
        {
            get { return dic; }
        }
        public IEnumerator<K> GetEnumerator()
        {
            return dic.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dic.Keys.GetEnumerator();
        }

    }
}