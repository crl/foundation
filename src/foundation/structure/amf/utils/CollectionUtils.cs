using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;

namespace foundation
{
    public abstract class CollectionUtils
	{
        private CollectionUtils() { }

		public static bool IsNullOrEmpty(ICollection collection)
		{
			if (collection != null)
			{
				return (collection.Count == 0);
			}
			return true;
		}
        
		public static IList CreateList(Type listType)
		{
			IList list;

			bool isReadOnlyOrFixedSize = false;

			if (listType.IsArray)
			{
                list = new List<object>();
				isReadOnlyOrFixedSize = true;
			}
			else if (typeof(IList).IsAssignableFrom(listType) && ReflectionUtils.IsInstantiatableType(listType))
			{
				list = (IList)Activator.CreateInstance(listType);
			}
            else if (ReflectionUtils.IsSubClass(listType, typeof(IList<>)))
            {
                list = CreateGenericList(ReflectionUtils.GetGenericArguments(listType)[0]) as IList;
            }
            else
            {
                throw new Exception(string.Format("Cannot create and populate list type {0}.", listType));
            }

			// create readonly and fixed sized collections using the temporary list
			if (isReadOnlyOrFixedSize)
			{
                if (listType.IsArray)
                {
                    list = ((List<object>)list).ToArray();
                }
                else if (ReflectionUtils.IsSubClass(listType, typeof(ReadOnlyCollection<>)))
                {
                    list = (IList)Activator.CreateInstance(listType, list);
                }
			}

			return list;
		}
        
		public static Array CreateArray(Type type, ICollection collection)
		{

            if (collection is Array)
            {
                return collection as Array;
            }
            List<object> tempList = new List<object>();
            foreach (object obj in collection)
            {
                tempList.Add(obj);
            }
            return tempList.ToArray();

		}

		#region GetSingleItem
  
		public static object GetSingleItem(IList list)
        {
            return GetSingleItem(list, false);
        }

        public static object GetSingleItem(IList list, bool returnDefaultIfEmpty)
        {
            if (list.Count == 1)
                return list[0];
            else if (returnDefaultIfEmpty && list.Count == 0)
                return null;
            else
                throw new Exception(string.Format("Expected single item in list but got {1}.", list.Count));
        }
		#endregion

		public static List<T> CreateList<T>(params T[] values)
		{
			return new List<T>(values);
		}

        public static bool IsNullOrEmpty<T>(ICollection<T> collection)
        {
            if (collection != null)
            {
                return (collection.Count == 0);
            }
            return true;
        }

        public static bool IsNullOrEmptyOrDefault<T>(IList<T> list)
        {
            if (IsNullOrEmpty<T>(list))
                return true;

            return ReflectionUtils.ItemsUnitializedValue<T>(list);
        }


        public static IList<T> Slice<T>(IList<T> list, int? start, int? end)
        {
            return Slice<T>(list, start, end, null);
        }

        public static IList<T> Slice<T>(IList<T> list, int? start, int? end, int? step)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (step == 0)
                throw new ArgumentException("Step cannot be zero.", "step");

            List<T> slicedList = new List<T>();

            // nothing to slice
            if (list.Count == 0)
                return slicedList;

            // set defaults for null arguments
            int s = step ?? 1;
            int startIndex = start ?? 0;
            int endIndex = end ?? list.Count;

            // start from the end of the list if start is negitive
            startIndex = (startIndex < 0) ? list.Count + startIndex : startIndex;

            // end from the start of the list if end is negitive
            endIndex = (endIndex < 0) ? list.Count + endIndex : endIndex;

            // ensure indexes keep within collection bounds
            startIndex = Math.Max(startIndex, 0);
            endIndex = Math.Min(endIndex, list.Count - 1);

            // loop between start and end indexes, incrementing by the step
            for (int i = startIndex; i < endIndex; i += s)
            {
                slicedList.Add(list[i]);
            }

            return slicedList;
        }

        public static void AddRange<T>(IList<T> initial, IEnumerable<T> collection)
        {
            if (initial == null)
                throw new ArgumentNullException("initial");

            if (collection == null)
                return;

            foreach (T value in collection)
            {
                initial.Add(value);
            }
        }
       
        public static List<T> Distinct<T>(List<T> collection)
        {
            List<T> distinctList = new List<T>();

            foreach (T value in collection)
            {
                if (!distinctList.Contains(value))
                    distinctList.Add(value);
            }

            return distinctList;
        }

        public static List<T> CreateList<T>(ICollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            T[] array = new T[collection.Count];
            collection.CopyTo(array, 0);

            return new List<T>(array);
        }
        
        public static bool ListEquals<T>(IList<T> a, IList<T> b)
        {
            if (a == null || b == null)
                return (a == null && b == null);

            if (a.Count != b.Count)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < a.Count; i++)
            {
                if (!comparer.Equals(a[i], b[i]))
                    return false;
            }

            return true;
        }

        public static object CreateGenericList(Type listType)
        {
            return ReflectionUtils.CreateGeneric(typeof(List<>), listType);
        }

        public static bool IsListType(Type type)
        {
            if (type.IsArray)
                return true;
            else if (typeof (IList).IsAssignableFrom(type))
                return true;
            else if (ReflectionUtils.IsSubClass(type, typeof (IList<>)))
                return true;
            else
                return false;
        }
	}
}