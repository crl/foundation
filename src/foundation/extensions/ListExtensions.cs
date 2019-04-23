using System.Collections.Generic;

namespace foundation
{
    public static class ListExtensions
    {
        /// <summary>
        /// 弹出第一项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T Shift<T>(this List<T> self)
        {
            if (self.Count > 0)
            {
                T item = self[0];
                self.RemoveAt(0);
                return item;
            }

            return default(T);
        }

        /// <summary>
        /// 复制一份数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this List<T> self)
        {
            List<T> result = new List<T>();
            foreach (T item in self)
            {
                result.Add(item);
            }
            return result;
        }

        public static List<T> Concat<T>(this List<T> self, List<T> other = null)
        {
            List<T> result = new List<T>();
            foreach (T item in self)
            {
                result.Add(item);
            }

            if (other != null)
            {
                foreach (T item in other)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 弹出最后一项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T Pop<T>(this List<T> self)
        {
            int last = self.Count-1;
            if (last >-1)
            {
                T item = self[last];
                self.RemoveAt(last);
                return item;
            }

            return default(T);
        }

        /// <summary>
        /// 从小到大排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<T> SortASC<T>(this List<T> self) where T: ISortWeight
        {
            self.Sort(AscSortFunc);
            return self;
        }
       /// <summary>
       /// 从大到小排序
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="self"></param>
       /// <returns></returns>
        public static List<T> SortDESC<T>(this List<T> self) where T : ISortWeight
        {
            self.Sort(DescSortFunc);
            return self;
        }

        /// <summary>
        /// 升序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int AscSortFunc<T>(T x, T y) where T : ISortWeight
        {
            long a = x.getSortWeight();
            long b = y.getSortWeight();

            if (a > b)
            {
                return 1;
            }else if (a == b)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int DescSortFunc<T>(T x, T y) where T : ISortWeight
        {
            long a = x.getSortWeight();
            long b = y.getSortWeight();

            if (a > b)
            {
                return -1;
            }
            else if (a == b)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}