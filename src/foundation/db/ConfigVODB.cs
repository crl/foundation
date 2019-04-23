using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace foundation
{
    public interface IConfigVODB
    {
        IEnumerator Deserialize(MemoryStream memoryStream,Action doNext);
        void Initialization();

        string GetFileName();
        void instanceFieldBind(object instance, FieldInfo fieldInfo, object value);
        void instanceFieldBind(object instance, string fieldKey, object value);

        object dbInstanceCreater();

        Type dbInstanceType { get; }
    }

    public class ConfigVODB<T> : ConfigVODB<T, string> where T : class, IConfigVO<string>, new()
    {
        public ConfigVODB(string fileName = "", string idField = ""):base(fileName,idField)
        {
        }
    }

    /// <summary>
    /// 单个配置数据库
    /// </summary>
    /// <typeparam name="T">存储类型</typeparam>
    /// <typeparam name="K">存储Key类型</typeparam>
    public class ConfigVODB<T,K> : IConfigVODB where T : class,IConfigVO<K>, new()
    {
        public static ConfigVODB<T,K> instance;
        public bool isBytesIsRawCSV = false;
        protected Dictionary<K, T> all = new Dictionary<K, T>();
        private string fileName;
        private string idField;

        public Func<T, K> IdGetFunc;

        public ConfigVODB(string fileName="",string idField="")
        {
            this.fileName = fileName;
            this.idField = idField;

            instance = this;
        }

        public virtual Dictionary<K, T> ALL()
        {
            return all;
        }

        public string GetFileName()
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            return fileName;
        }

        public virtual object dbInstanceCreater()
        {
            return new T();
        }

        public virtual Type dbInstanceType
        {
            get { return typeof(T); }
        }

        public virtual IEnumerator Deserialize(MemoryStream memoryStream, Action doNext)
        {
            if (memoryStream == null)
            {
                doNext();
                yield break;
            }
            Type type = typeof (T);
            FieldInfo idFieldInfo = null;
            if (string.IsNullOrEmpty(idField) == false)
            {
                idFieldInfo = type.GetField(idField);
            }
            long st = DateTime.Now.Ticks;
            IList list = null;
            if (isBytesIsRawCSV)
            {
                list = new List<T>();
                ByteArray reader = new ByteArray(memoryStream);
                IList rawList = (IList) reader.ReadObject();
                IList propertys = (IList) rawList[0];
                int propertyLen = propertys.Count;

                List<int> hasPropertyList = new List<int>();
                List<FieldInfo> hasPropertyFieldInfoList = new List<FieldInfo>();
                for (int j = 0; j < propertyLen; j++)
                {
                    FieldInfo fieldInfo = type.GetField((string) propertys[j]);
                    if (fieldInfo != null)
                    {
                        hasPropertyList.Add(j);
                        hasPropertyFieldInfoList.Add(fieldInfo);
                    }
                }
                propertyLen = hasPropertyList.Count;

                IList itemData;
                int dataLen = rawList.Count;

                for (int i = 1; i < dataLen; i++)
                {
                    itemData = (IList) rawList[i];
                    T instance = new T();
                    for (int j = 0; j < propertyLen; j++)
                    {
                        FieldInfo fieldInfo = hasPropertyFieldInfoList[j];
                        int index = hasPropertyList[j];
                        object value = itemData[index];
                        if (value == null)
                        {
                            continue;
                        }
                        this.instanceFieldBind(instance, fieldInfo, value);
                    }
                    list.Add(instance);


                    if (i%1000 == 0)
                    {
                        yield return null;
                    }

                }
            }
            else
            {
                SimpleAMFReader simpleAMFReader = new SimpleAMFReader();
                yield return simpleAMFReader.readAMFList(memoryStream, this);
                list = simpleAMFReader.getResult();
            }

            int len = 0;

            if (list != null)
            {
                len = list.Count;
            }
            T _pp;
            K id;
            Type keyType = typeof (K);
            for (int i = 0; i < len; i++)
            {
                _pp = list[i] as T;
                if (_pp == null)
                {
                    Debug.Log(fileName +" type Error");
                    break;
                }
                    
                if (IdGetFunc != null)
                {
                    id = IdGetFunc(_pp);
                }
                else if (idFieldInfo != null)
                {
                    id = (K)Convert.ChangeType(idFieldInfo.GetValue(_pp),keyType);
                }
                else
                {
                    id = _pp.GetID();
                }
                if (id==null || id.ToString()=="")
                {
                    continue; 
                }
                addDBItem(id, _pp);
            }

            if (Application.isEditor)
            {
                Debug.Log(fileName + " hasCount:" + len + "\tsinceTime:" + (DateTime.Now.Ticks - st));
            }
            doNext();
        }

        protected virtual bool addDBItem(K id, T cfgvo)
        {
            if (all.ContainsKey(id) == false)
            {
                all.Add(id, cfgvo);
                return true;
            }
            return false;
        }

        public virtual void Initialization()
        {
        }
        public virtual bool instanceResolveFieldBind(T instance, FieldInfo fieldKey, object value)
        {
            return false;
        }
        public virtual void instanceFieldBind(object instance, string fieldKey, object value)
        {

        }
        public virtual void instanceFieldBind(object instance, FieldInfo fieldInfo, object value)
        {
            if (value == null)
            {
                return;
            }
            if (value is string && string.IsNullOrEmpty((string)value))
            {
                TypeCode code = Type.GetTypeCode(fieldInfo.FieldType);
                if (code == TypeCode.String)
                {
                    value = "";
                }
                else if (code == TypeCode.Int32)
                {
                    value = 0;
                }
            }

            try
            {
                value = Convert.ChangeType(value, fieldInfo.FieldType);
                fieldInfo.SetValue(instance, value);
            }
            catch (Exception ex)
            {
                bool b = instanceResolveFieldBind((T)instance, fieldInfo, value);
                if (Application.isEditor && b==false)
                {
                    string msg = StringUtil.substitute("{0} [{1}:{2}={3}\tError:{4}", fileName, fieldInfo.Name,
                        fieldInfo.FieldType, value, ex.Message);
                    DebugX.LogWarning(msg);
                }
            }
        }

        public static T Get(K id)
        {
            return instance.get(id);
        }

        public static List<T> AllListValue()
        {
            List<T> result = new List<T>();
            foreach (T item in instance.ALL().Values)
            {
                if (item != null)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static T[] AllArrayValue()
        {
            Dictionary<K, T>.ValueCollection all = instance.all.Values;
            T[] result = new T[all.Count];
            int i = 0;
            foreach (T v in all)
            {
                result[i] = v;
                i++;
            }
            return result;
        }

        public virtual T get(K id)
        {
            if (id is string)
            {
                if (string.IsNullOrEmpty(id as string))
                {
                    return default(T);
                }
            }
            if (id == null)
            {
                return default(T);
            }

            T result;
            all.TryGetValue(id, out result);
            return result;
        }
        public virtual T Clone(K id)
        {
            return get(id);
        }
    }
}