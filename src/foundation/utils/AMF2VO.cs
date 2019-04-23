using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace foundation
{
    public class AMF2VO
    {
        public static List<T> format<T>(ByteArray bytesArray) where T : new()
        {
            object[] rawData = bytesArray.ReadObject() as object[];
            return format<T>(rawData);
        }

        public static List<T> format<T>(object[] rawListData) where T : new()
        {
            List<T> result = new List<T>();
            Type type = typeof (T);
            FieldInfo[] infos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (object item in rawListData)
            {
                IDictionary itemMap = item as IDictionary;
                T _pp = Activator.CreateInstance<T>();
                object value;
                foreach (FieldInfo fieldInfo in infos)
                {
                    value = itemMap[fieldInfo.Name];
                    if (value is string && string.IsNullOrEmpty((string) value))
                    {
                        value = "";
                    }
                    value = Convert.ChangeType(value, fieldInfo.FieldType);
                    fieldInfo.SetValue(_pp, value);
                }
                result.Add(_pp);
            }
            return result;
        }
    }
}