using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace foundation
{
    public class ObjectUtils
    {
        public static string getString(string v,string def="")
        {
            if (string.IsNullOrEmpty(v))
            {
                return def;
            }
            return v;
        }
        public static string getString(object v, string def = "")
        {
            if (v==null)
            {
                return def;
            }

            if (v is string)
            {
                return getString((string) v, def);
            }

            return v.ToString();
        }

        public static object getValue(object from, string property)
        {
            if (from is ASObject)
            {
                object value;
                (from as ASObject).TryGetValue(property, out value);
                return value;
            }

            Type fromType = from.GetType();
            FieldInfo fieldInfo = fromType.GetField(property, BindingFlags.Instance | BindingFlags.Public);

            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(from);
            }

            return null;
        }

        public static string getStringValue(object from, string property)
        {
            object value = getValue(from, property);

            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }

        public static int getIntValue(object from, string property)
        {
            object value = getValue(from, property);

            if (value == null)
            {
                return 0;
            }

            return int.Parse(value.ToString());
        }

        public static float getFloatValue(object from, string property)
        {
            object value = getValue(from, property);

            if (value == null)
            {
                return 0;
            }

            return float.Parse(value.ToString());
        }

        public static void copyFrom(object toVO, object from)
        {
            if (toVO == null || from==null)
            {
                return;
            }
            Type toType = toVO.GetType();
            int len = 0;
            FieldInfo toInfo;
            object value = null;
            if (from is ASObject)
            {
                ASObject fromData = (ASObject)from;
                string[] keys = fromData.Keys.ToArray();
                len = keys.Count();
                for (int i = 0; i < len; i++)
                {
                    string name = keys[i];
                    toInfo = toType.GetField(name);
                    if (toInfo != null)
                    {
                        fromData.TryGetValue(name, out value);
                        setValue(toInfo, name, toVO, value);
                    }
                }

                return;
            }

            Type fromType = from.GetType();
            FieldInfo[] infos = fromType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            len = infos.Length;
            for (int i = 0; i < len; i++)
            {
                FieldInfo info = infos[i];
                toInfo = toType.GetField(info.Name);
                if (toInfo != null)
                {
                    value = info.GetValue(from);

                    setValue(toInfo, info.Name, toVO, value);
                }
            }

        }

        /// <summary>
        /// 深复制
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static object deepClone(object p)
        {
            BinaryFormatter Formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            MemoryStream stream = new MemoryStream();
            Formatter.Serialize(stream, p);
            stream.Position = 0;
            object clonedObj = Formatter.Deserialize(stream);
            stream.Close();
            return clonedObj;
        }

        private static void setValue(FieldInfo toInfo, string name, object obj, object value)
        {
            try
            {
                toInfo.SetValue(obj, value);
            }
            catch (Exception)
            {
                try
                {
                    if (toInfo.FieldType == typeof(BigInteger))
                    {
                        if (value is string && string.IsNullOrEmpty((string)value))
                        {
                            value = 0;
                        }

                        value = BigIntegerHelper.FromScientific(value.ToString());
                    }


                    value = Convert.ChangeType(value, toInfo.FieldType);
                    toInfo.SetValue(obj, value);
                }
                catch (Exception e)
                {
                    DebugX.Log(obj.ToString() + ":" + name + " 类型转换失败,需要为:" + toInfo.FieldType + " value:" + value + "\n" +
                              e.Message);
                }
            }

        }

    }
}
