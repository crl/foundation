using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    /// <summary>
    /// 像as一样操作的dictionary(默认实现)
    /// </summary>
    [Serializable]
    public class ASDictionary : ASDictionary<string,object>
    {
        new public static ASDictionary Get()
        {
            return new ASDictionary();
        }

        public string getString(string key,string def="")
        {
            if (string.IsNullOrEmpty(key))
            {
                return def;
            }
            object o=this[key];
            if (o != null)
            {
                return o.ToString();
            }
            else
            {
                return def;
            }
        }

        public T getValue<T>(string key) where T:class 
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }
            object o = this[key];
            if (o != null)
            {
                return o as T;
            }
            else
            {
                return default(T);
            }
        }
        public Vector3 getVector3(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return Vector3.zero;
            }
            object o = this[key];
            if (o is Vector3)
            {
                return (Vector3)o;
            }
            else
            {
                return Vector3.zero;
            }
        }
        public int getInt(string key, int def = -1)
        {
            if (string.IsNullOrEmpty(key))
            {
                return def;
            }
            object o = this[key];
            if (o != null)
            {
                int i;
                if (int.TryParse(o.ToString(), out i))
                {
                    return i;
                }
                return def;
            }
            else
            {
                return def;
            }
        }
        
        public uint getUint(string key, uint def = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                return def;
            }
            object o = this[key];
            if (o != null)
            {
                uint i;
                if (uint.TryParse(o.ToString(), out i))
                {
                    return i;
                }
                return def;
            }
            else
            {
                return def;
            }
        }

        public float getFloat(string key, float def = 0.0f)
        {
            if (string.IsNullOrEmpty(key))
            {
                return def;
            }
            object o = this[key];
            if (o != null)
            {
                float i;
                if (float.TryParse(o.ToString(), out i))
                {
                    return i;
                }
                return def;
            }
            else
            {
                return def;
            }
        }

        public bool getBool(string key, bool def = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return def;
            }
            object o = this[key];
            if (o != null)
            {
                bool i;
                if (Boolean.TryParse(o.ToString(), out i))
                {
                    return i;
                }
                return def;
            }
            else
            {
                return def;
            }
        }
    }
    [Serializable]
    public class ASDictionary<V> : ASDictionary<string, V>
    {
        new public static ASDictionary<string, V> Get()
        {
            return new ASDictionary<string, V>();
        }
    }

    /// <summary>
    ///  像as一样操作的dictionary
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public class ASDictionary<K,V>: IEnumerable<K>
    {
        private Dictionary<K,V> dic=new Dictionary<K, V>();



        public static ASDictionary<K, V> Get()
        {
            return new ASDictionary<K, V>();
        }
        public bool ContainsKey(K value)
        {
            return dic.ContainsKey(value);
        }

        public bool ContainsValue(V value)
        {
            return dic.ContainsValue(value);
        }

        public bool Remove(K key)
        {
            if (dic.ContainsKey(key))
            {
                return dic.Remove(key);
            }
            return false;
        }

        public void Add(K key, V value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key]=value;
                return;
            }
            dic.Add(key, value);
        }

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

                if (dic.ContainsKey(key))
                {
                    dic[key] = value;
                    return;
                }

                dic.Add(key, value);

            }
        }

        public void Clear()
        {
            dic.Clear();
        }

        public Dictionary<K, V>.ValueCollection Values
        {
            get { return dic.Values; }
        }
        public Dictionary<K, V>.KeyCollection Keys
        {
            get { return dic.Keys; }
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