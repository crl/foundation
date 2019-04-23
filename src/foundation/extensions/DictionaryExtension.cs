using System.Collections.Generic;

namespace foundation
{
    public static class DictionaryExtension
    {
        public static T Get<T>(this Dictionary<string,object> self, string key)
        {
            object o = null;

            if (self.TryGetValue(key, out o))
            {
                return (T)o;
            }
            return default(T);
        }

        public static T Get<T>(this Dictionary<int, object> self, int key)
        {
            object o = null;

            if (self.TryGetValue(key, out o))
            {
                return (T)o;
            }
            return default(T);
        }

        public static T Get<T>(this Dictionary<string, T> self, string key)
        {
            T o = default(T);
            if (self.TryGetValue(key, out o))
            {
                return o;
            }
            return default(T);
        }
        public static T Get<T>(this Dictionary<int, T> self, int key)
        {
            T o = default(T);
            if (self.TryGetValue(key, out o))
            {
                return o;
            }
            return default(T);
        }
    }
}