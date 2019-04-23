using System;
using System.Collections.Generic;

namespace foundation
{
    public enum SortDirection
    {
        /// <summary>
        /// 从小到大
        /// </summary>
        ASC = 0,
        /// <summary>
        /// 从大到小
        /// </summary>
        DESC,
        RANDOM
    };

    public interface ISortWeight
    {
        long getSortWeight();
    }

    public class SortComparer<T> : IComparer<T> where T: IComparable<T>
    {
        public SortDirection sortDirection = SortDirection.ASC;

        public int Compare(T x, T y)
        {
            switch (sortDirection)
            {
                case SortDirection.ASC:
                    return x.CompareTo(y);
                case SortDirection.DESC:
                    return y.CompareTo(x);
                case SortDirection.RANDOM:
                    return UnityEngine.Random.Range(0.0f, 1.0f) < .5 ? -1 : 1;
            }
            return 0;
        }
    }
}