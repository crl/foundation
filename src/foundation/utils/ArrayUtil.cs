using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace foundation
{



//C# Array 数组辅助类.
    public class ArrayUtil
    {
        public ArrayUtil()
        {
        }

        public static void AddItem<T>(ref T[] arr, T obj)
        {
            System.Array.Resize(ref arr, arr.Length + 1);
            arr[arr.Length - 1] = obj;
        }

        public static T[] AddItemAt<T>(T[] source, int index, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("ArrayUtil.AddItemAt::source == null");
            }

            if (index < 0 || index > source.Length)
            {
                throw new ArgumentOutOfRangeException("ArrayUtil.AddItemAt::index < 0 || index > source.Length");
            }

            T[] result = new T[source.Length + 1];

            for (int i = 0; i < index; ++i)
            {
                result[i] = source[i];
            }

            result[index] = item;

            for (int i = index; i < source.Length; ++i)
            {
                result[i + 1] = source[i];
            }

            return result;
        }

        public static void RemoveItem<T>(ref T[] arr, T obj) where T : class
        {
            T[] a = { };

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != obj)
                {
                    AddItem(ref a, arr[i]);
                }
            }
            arr = a;
        }

        public static void RemoveItemAt<T>(ref T[] arr, int index)
        {
            Splice(ref arr, index, 1);
        }

        public static void RemoveItems<T>(ref T[] arr, int startIndex, int delNum)
        {
            T[] teamArr = new T[] { };

            for (int i = 0; i < arr.Length; i++)
            {
                if (i < startIndex || i > (startIndex + delNum - 1))
                {
                    AddItem(ref teamArr, arr[i]);
                }
            }

            arr = teamArr;
        }

        /// <summary>
        /// 获取数组中的某段.返回副本,原始数组不会被改变.Get the items.
        /// </summary>
        public static T[] GetItems<T>(T[] source, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(source, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        public static int IndexOf(Array arr, object item)
        {
            return Array.IndexOf(arr, item);
        }

        /// <summary>
        /// Checks whether the specified array is a null reference or an empty array.
        /// </summary>
        public static bool IsNullOrEmpty(Array array)
        {
            bool isNullOrEmtpy = (array == null) || (array.Length == 0);
            return isNullOrEmtpy;
        }

        /// <summary>
        /// Checks whether the specified collection is a null reference or an empty collection.
        /// </summary>
        public static bool IsNullOrEmpty(IEnumerable collection)
        {
            if (collection == null)
                return true;

            IEnumerator enumerator = collection.GetEnumerator();
            bool isEmpty = !enumerator.MoveNext();
            return isEmpty;
        }

        public static T[] Copy<T>(T[] array)
        {
            return (array.Length > 0) ? (T[]) array.Clone() : array;
        }

        /// <summary>
        /// 复制数组里的一段. Get the array slice between the two indexes.
        /// 原始数组将不会改变.
        /// ... Inclusive for start index, exclusive for end index.
        /// </summary>
        public static T[] Slice<T>(T[] source, int start, int end)
        {
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            T[] res = new T[len];

            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }

//	public static T[] Slice<T>(this T[] arr, uint indexFrom, uint indexTo) 
//	{
//	    if (indexFrom > indexTo) {
//	        throw new ArgumentOutOfRangeException("indexFrom is bigger than indexTo!");
//	    }
//	
//	    uint length = indexTo - indexFrom;
//	    T[] result = new T[length];
//	    Array.Copy(arr, indexFrom, result, 0, length);
//	
//	    return result;
//	}

        /// <summary>
        /// 删除数组里的一段，返回删除掉的元素组成的数组.Splice the specified source, start and deleteCount.
        /// 原始数组将被改变.
        /// </summary>
        public static T[] Splice<T>(ref T[] source, int start, int deleteCount)
        {
            //Array.Copy(sourceArr 原数组,sourceIndex 原数组起索引,destinationArr 目标数组,destinationIndex 目标数组起始索引,length 复制总数);
            int delCount = 0;

            if (source.Length - start < deleteCount)
            {
                delCount = source.Length - start;
            }
            else if (source.Length == deleteCount)
            {
                delCount = source.Length;
            }
            else
            {
                delCount = deleteCount;
            }

            T[] arrCopyTo = new T[delCount]; //删除掉的元素组成的数组.
            Array.Copy(source, start, arrCopyTo, 0, delCount);

            //以下是修改原始数组(从中删除元素).
            T[] arr1 = new T[start];
            Array.Copy(source, 0, arr1, 0, start);
            int len = source.Length - (start + delCount);
            T[] arr2 = new T[len];
            Array.Copy(source, start + delCount, arr2, 0, len);
            source = Concat(arr1, arr2);

            return arrCopyTo;
        }

        /// <summary>
        /// 合并两个数组.Concat the arr1 and arr2.
        /// </summary>
        public static T[] Concat<T>(T[] arr1, T[] arr2)
        {
            if (arr1 == null)
            {
                throw new ArgumentNullException("arr1 == null");
            }
            if (arr2 == null)
            {
                throw new ArgumentNullException("arr2 == null");
            }
            int oldLen = arr1.Length;
            Array.Resize<T>(ref arr1, arr1.Length + arr2.Length);
            Array.Copy(arr2, 0, arr1, oldLen, arr2.Length);
            return arr1;
        }

        /// <summary>
        /// 打印一个数组里所有数据.
        /// </summary>
        public static void PrintArray<T>(T[] source)
        {
            string s = "";
            foreach (var item in source)
            {
                s += item.ToString() + ",";
            }

        }

        /// <summary>
        /// 比较两个 List，如果数据集一样则返回 true （该比较会忽略数据集的顺序）
        /// </summary>
        public static bool CompareList<T>(List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            foreach (var item in list1)
            {
                if (!list2.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }
    }
}