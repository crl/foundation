using System.Collections.Generic;

namespace foundation
{
    public static class ListPool<T> where T:new()
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>();

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
            toRelease.Clear();
        }
    }
}
