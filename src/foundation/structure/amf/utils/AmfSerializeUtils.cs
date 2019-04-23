using System;
using System.Collections;
using System.Collections.Generic;

namespace foundation
{
    public class AmfSerializeUtils
    {
		public static int readU29(IDataInput bytes) {
            int value;

            // Each byte must be treated as unsigned
            int b = bytes.ReadByte() & 0xFF;

            if (b < 128)
                return b;

            value = (b & 0x7F) << 7;
            b = bytes.ReadByte() & 0xFF;

            if (b < 128)
                return (value | b);

            value = (value | (b & 0x7F)) << 7;
            b = bytes.ReadByte() & 0xFF;

            if (b < 128)
                return (value | b);

            value = (value | (b & 0x7F)) << 8;
            b = bytes.ReadByte() & 0xFF;

            return (value | b);
        }


        public static T[] getList<T>(object v)
        {
            if (v is T[])
            {
                return (T[])v;
            }
            T[] list = null;
            if (v is IList)
            {
                IList o = (IList) v;
                list = new T[o.Count];
                int i = 0;
                foreach (object item in o)
                {
                    list[i++] = (T)Convert.ChangeType(item, typeof(T));
                }
            }
            return list;
        }

        public static Dictionary<T, S> getDictionary<T, S>(object dic)
        {
            Dictionary<T, S> result = null;

            if (dic is Dictionary<T, S>)
            {
                return (Dictionary<T, S>)dic;
            }

            if (dic is Dictionary<object, object>)
            {
                Type tt = typeof (T);
                Type st = typeof (S);
                result = new Dictionary<T, S>();
                Dictionary<object, object> mapping = (Dictionary<object, object>) dic;

                object k;
                object v;
                foreach (object item in mapping.Keys)
                {
                    k = Convert.ChangeType(item, tt);
                    v = Convert.ChangeType(mapping[item], st);
                    result.Add((T) k, (S) v);
                }
            }
            return result;
        }
    }
}
