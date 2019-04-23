using System;
using System.Reflection;

namespace foundation
{
    public interface IConfigVO
    {
        T GetValue<T>(string key);
        int GetInt(string key);
        string GetString(string key);
    }

    public interface IConfigVO<K>:IConfigVO
    {
        K GetID();
    }

    [System.Serializable]
    public class ConfigVO : ConfigVO<string>
    {
        
    }
    [System.Serializable]
    public class ConfigVO<K>:IConfigVO<K>
    {
        public K id;

        public virtual T GetValue<T>(string key)
        {
            FieldInfo fi = GetCacheField(GetType(),key);
            if (fi == null)
            {
                return default(T);
            }

            return (T)fi.GetValue(this);
        }


        public int GetInt(string key)
        {
            return GetValue<int>(key);
        }

        public string GetString(string key)
        {
            return GetValue<string>(key);
        }

        public static FieldInfo GetCacheField(Type cfg,string key)
        {
            return cfg.GetField(key);
        }

        public K GetID()
        {
            return id;
        }
    }
}