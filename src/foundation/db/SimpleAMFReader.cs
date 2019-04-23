using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace foundation
{
    public class SimpleAMFReader
    {
        private Dictionary<string, FieldInfo> fieldInfosDic = new Dictionary<string, FieldInfo>();
        private Dictionary<string, object> hashtable = null;
        private IList result;

        public IEnumerator readAMFList(MemoryStream memoryStream, IConfigVODB configVodb)
        {
            AMFReader reader = new AMFReader(memoryStream);
            byte typeCode = reader.ReadByte();
            //DebugX.Log("typeCode:"+typeCode);
            if (typeCode != 9)
            {
                yield break;
            }
            Type type = configVodb.dbInstanceType;
            int handle = reader.ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0);
            handle = handle >> 1;
            if (inline)
            {
                hashtable = null;
                string key = reader.ReadAMF3String();
                object value;
                while (key != null && key != string.Empty)
                {
                    if (hashtable == null)
                    {
                        hashtable = new Dictionary<string, object>();
                        reader.AddAMF3ObjectReference(hashtable);
                    }
                    value = reader.ReadAMF3Data();
                    hashtable.Add(key, value);
                    key = reader.ReadAMF3String();
                }
                //Not an associative array
                if (hashtable == null)
                {
                    object[] array = new object[handle];
                    reader.AddAMF3ObjectReference(array);

                    //long startParserTime = DateTime.Now.Ticks;
                    FieldInfo[] fieldInfos = type.GetFields();
                    fieldInfosDic.Clear();

                    string fieldName;
                    for (int i = 0, len = fieldInfos.Length; i < len; i++)
                    {
                        fieldName = fieldInfos[i].Name;
                        if (fieldInfosDic.ContainsKey(fieldName) == false)
                        {
                            fieldInfosDic.Add(fieldName, fieldInfos[i]);
                        }
                    }

                    for (int i = 0; i < handle; i++)
                    {
                        typeCode = reader.ReadByte();
                        if (typeCode == 10)
                        {
                            array[i] = ReadAMF3Object(reader,configVodb);
                        }
                        else
                        {
                            array[i] = reader.ReadAMF3Data(typeCode);
                        }

                        if (i%1000 == 0)
                        {
                            yield return null;
                        }
                    }
                    //DebugX.Log("tt:" + (DateTime.Now.Ticks - startParserTime) / 10000);
                    result = array;
                }
                else
                {
                    for (int i = 0; i < handle; i++)
                    {
                        value = reader.ReadAMF3Data();
                        hashtable.Add(i.ToString(), value);
                    }
                    result = (IList) hashtable;
                }
            }
            else
            {
                result = (IList) reader.ReadAMF3ObjectReference(handle);
            }
        }

        private static FieldInfo fieldInfo = null;
        private static ClassDefinition classDefinition = null;
        private object ReadAMF3Object(AMFReader reader,IConfigVODB configVodb)
        {
            int handle = reader.ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0); handle = handle >> 1;
            if (!inline)
            {
                //An object reference
                return reader.ReadAMF3ObjectReference(handle);
            }
           
            bool inlineClassDef = ((handle & 1) != 0); handle = handle >> 1;
            if (inlineClassDef)
            {
                //inline class-def
                string typeIdentifier = reader.ReadAMF3String();
                //flags that identify the way the object is serialized/deserialized
                bool externalizable = ((handle & 1) != 0);
                handle = handle >> 1;
                bool dynamic  = ((handle & 1) != 0);
                handle = handle >> 1;
                ClassMember[] members = null;
                if (handle > 0)
                {
                    members = new ClassMember[handle];
                    for (int i = 0; i < handle; i++)
                    {
                        string name = reader.ReadAMF3String();
                        ClassMember classMember = new ClassMember(name, BindingFlags.Default, MemberTypes.Custom, null);
                        members[i] = classMember;
                    }
                }
                classDefinition = new ClassDefinition(typeIdentifier, members, externalizable, dynamic);
                reader.AddClassReference(classDefinition);
            }
            else
            {
                classDefinition = reader.ReadClassReference(handle);
            }

            object instance = configVodb.dbInstanceCreater();
            reader.AddAMF3ObjectReference(instance);

            for (int i = 0; i < classDefinition.MemberCount; i++)
            {
                string key = classDefinition.Members[i].Name;

                object value = reader.ReadAMF3Data();
                reader.SetMember(instance, key, value);
            }

            if (classDefinition.IsDynamic)
            {
                string key = reader.ReadAMF3String();
                object value;
                while (key != null && key != string.Empty)
                {
                    value = reader.ReadAMF3Data();
                   
                    if (fieldInfosDic.TryGetValue(key, out fieldInfo))
                    {
                        configVodb.instanceFieldBind(instance, fieldInfo, value);
                    }else if (fieldInfosDic.TryGetValue("__" + key, out fieldInfo))
                    {
                        configVodb.instanceFieldBind(instance, fieldInfo, value);
                    }
                    else
                    {
                        configVodb.instanceFieldBind(instance, key, value);
                    }
                    key = reader.ReadAMF3String();
                }
            }

            return instance;
        }

        public IList getResult()
        {
            return result;
        }
    }
}