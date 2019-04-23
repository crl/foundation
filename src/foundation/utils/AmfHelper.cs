using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace foundation
{
    public class AmfHelper
    {
        public static bool save(object o, string fullPath)
        {
            ByteArray bytesArray = new ByteArray();
            try
            {
                bytesArray.WriteObject(o);
                bytesArray.Deflate();

                byte[] bytes = bytesArray.ToArray();

                using (FileStream fileStream = File.Open(fullPath, FileMode.Create))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return false;
            }
            return true;
        }


        public static void WriteRect(IDataOutput output, Rect rect)
        {
            output.WriteFloat(rect.x);
            output.WriteFloat(rect.y);

            output.WriteFloat(rect.width);
            output.WriteFloat(rect.height);
        }

        public static Rect ReadRect(IDataInput input)
        {
            Rect rect=new Rect();
            rect.x= input.ReadFloat();
            rect.y=input.ReadFloat();

            rect.width= input.ReadFloat();
            rect.height= input.ReadFloat();

            return rect;
        }

        public static void WriteVector2(IDataOutput output, Vector2 v)
        {
            output.WriteFloat(v.x);
            output.WriteFloat(v.y);
        }

        public static Vector2 ReadVector2(IDataInput input)
        {
            Vector2 v = new Vector2();
            v.x = input.ReadFloat();
            v.y = input.ReadFloat();
            return v;
        }

        public static void WriteVector3(IDataOutput output, Vector3 v)
        {
            output.WriteFloat(v.x);
            output.WriteFloat(v.y);
            output.WriteFloat(v.z);
        }

        public static Vector3 ReadVector3(IDataInput input)
        {
            Vector3 v = new Vector3();
            v.x = input.ReadFloat();
            v.y = input.ReadFloat();
            v.z = input.ReadFloat();
            return v;
        }


        public static bool saveByteArray(ByteArray bytesArray, string fullPath)
        {
            if (bytesArray == null)
            {
                return false;
            }
            try
            {
                byte[] bytes = bytesArray.ToArray();
                using (FileStream fileStream = File.Open(fullPath, FileMode.Create))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return false;
            }
            return true;
        }

        public static object get(string fullPath)
        {
            ByteArray bytesArray = getByteArray(fullPath);
            if (bytesArray == null)
            {
                return null;
            }
            return bytesArray.ReadObject();
        }

        public static ByteArray getByteArray(string fullPath)
        {
            if (File.Exists(fullPath) == false)
            {
                return null;
            }

            byte[] bytes;

            using (FileStream fileStream = File.OpenRead(fullPath))
            {
                bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);

            }

            if (bytes != null)
            {
                ByteArray bytesArray = new ByteArray(bytes);
                try
                {
                    bytesArray.Inflate();
                }
                catch (Exception)
                {
                    bytesArray.Position = 0;
                }

                return bytesArray;
            }

            return null;
        }

        public static Vector2 getVector2(object o)
        {
            IDictionary dic=o as IDictionary;
            if (dic != null)
            {
                float x=(float)Convert.ChangeType(dic["x"], TypeCode.Single);
                float y = (float)Convert.ChangeType(dic["y"], TypeCode.Single);

                return new Vector2(x,y);
            }

            return Vector2.zero;
        }

        public static void copyIList(IList to, IList from,TypeCode code)
        {
            if (from == null)
            {
                return;
            }
            foreach (object item in from)
            {
                if (item == null)
                {
                    continue;
                }
                to.Add(Convert.ChangeType(item,code));
            }
        }

        public static void copyIList(IList to, IList from)
        {
            if (from == null)
            {
                return;
            }

            foreach (object item in from)
            {
                if (item == null)
                {
                    continue;
                }
                to.Add(item);
            }
        }

        public static void copyDictionary(IDictionary to, IDictionary from)
        {
            if (from == null)
            {
                return;
            }
            foreach (object key in from.Keys)
            {
                object item = from[key];
                if (item == null)
                {
                    continue;
                }
                to.Add(key,item);
            }
        }

        public static void copyVector2Dictionary(IDictionary to, IDictionary from)
        {
            if (from == null)
            {
                return;
            }
            foreach (object key in from.Keys)
            {
                object item = from[key];
                if (item == null)
                {
                    continue;
                }
                Vector2 v = getVector2(item);
                to.Add(key, v);
            }
        }

    }
}