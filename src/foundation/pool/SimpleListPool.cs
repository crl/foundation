using System.Collections.Generic;

namespace foundation
{
    /// <summary>
    /// 列表池 临时列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class SimpleListPool<T>
    {
        private static Stack<List<T>> Pools = new Stack<List<T>>();
        public static List<T> Get()
        {
            if (Pools.Count > 0)
            {
                return Pools.Pop();
            }

            return new List<T>();
        }

        public static void Release(List<T> value)
        {
            if (Pools.Count < 100)
            {
                value.Clear();
                Pools.Push(value);
            }
        }
    }
}