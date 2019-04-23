using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace foundation
{
    public sealed class ObjectFactory
	{
        private static volatile ObjectFactory _instance;
        private static readonly object SyncRoot = new Object();

        private readonly Dictionary<string, Type> _typeCache;
        private readonly Dictionary<Type, string> _invertTypeCache;

	    private ObjectFactory() 
        {
            _typeCache = new Dictionary<string, Type>();
	        _invertTypeCache = new Dictionary<Type, string>();
        }

        public static void registerClassAlias(string fullName,Type type){
            Instance._typeCache[fullName]=type;
            Instance._invertTypeCache[type] = fullName;
        }
		
		public static void registerClassAlias<T>(string fullName,bool forceReplace=false)
		{
		    Type clazz;

            if(Instance._typeCache.TryGetValue(fullName,out clazz))
		    {
		        if (forceReplace == false)
		        {
		            return;
		        }
                clazz = typeof(T);
            }
            else
		    {
                clazz = typeof(T);
            }

		    Instance._typeCache[fullName]=clazz;
            Instance._invertTypeCache[clazz] = fullName;
		}
		

        public static ObjectFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot) 
                    {
                        if (_instance == null)
                            _instance = new ObjectFactory();
                    }
                }
                return _instance;
            }
        }

		public Type InternalLocate(string typeName)
		{
			if( string.IsNullOrEmpty(typeName) ){
				return null;
            }
            Type type;
            if (!_typeCache.TryGetValue(typeName, out type))
            {
                type = DomainLocate(typeName);
                if (type != null){
                    _typeCache[typeName] = type;
                }
            }
            return type;
		}

        static public Type DomainLocate(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            Assembly[] assemblies = GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                Type type = assembly.GetType(typeName, false);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }
        static public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }


        internal void AddTypeToCache(Type type)
		{
            if (type != null)
                _typeCache[type.FullName] = type;
		}

		public bool ContainsType(string typeName)
		{
            if (string.IsNullOrEmpty(typeName))
                return false;
            return _typeCache.ContainsKey(typeName);
		}

        public object InternalCreateInstance(Type type)
		{
            return InternalCreateInstance(type, null);
		}

        public object InternalCreateInstance(string typeName)
        {
            return InternalCreateInstance(typeName, null);
        }

        public object InternalCreateInstance(string typeName, object[] args)
        {
            Type type = InternalLocate(typeName);
            return InternalCreateInstance(type, args);
        }

        public object InternalCreateInstance(Type type, object[] args)
		{
            if (type != null)
            {
                if (type.IsAbstract && type.IsSealed){
                    return type;
                }
                try
                {
                    return Activator.CreateInstance(type,args);
                }
                catch (Exception exception)
                {
                    string msg = String.Format("amf Activator.CreateInstance {0} error type:{1}", type.FullName,
                        exception.Message);
                    DebugX.LogWarning(msg);
                }
            }
			return null;
		}

        static public Type Locate(string type)
        {
            return Instance.InternalLocate(type);
        }

        static public object CreateInstance(Type type)
        {
            return Instance.InternalCreateInstance(type);
        }

        static public object CreateInstance(string type)
        {
            return Instance.InternalCreateInstance(type);
        }

        static public object CreateInstance(Type type, object[] args)
        {
            return Instance.InternalCreateInstance(type, args);
        }

        public static string GetCustomClass(Type type)
        {
            return Instance.getCustomClass(type);
        }

        private string getCustomClass(Type type)
        {
            if (type==null)
            {
                return null;
            }
            string t;
            if (_invertTypeCache.TryGetValue(type, out t))
            {
                if (string.IsNullOrEmpty(t))
                {
                    _invertTypeCache[type] = t = type.FullName;
                }
            }
            else { 
                _invertTypeCache[type] =t= type.FullName;
            }
            
            return t;
        }
    }
}
