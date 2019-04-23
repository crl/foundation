using System;
using System.Collections.Generic;

namespace foundation
{
    /// <summary>
    ///  类的管理工具,可注册类的别名
    ///  供MVC框架使用,功能强大
    /// </summary>
    public class Singleton
    {
        private static Dictionary<string, Type> uniqueMap = new Dictionary<string, Type>();
        private static Dictionary<string, Object> uniqueInstanceMap = new Dictionary<string, object>();

        private static Dictionary<string, Type> __classMap = new Dictionary<string, Type>();       
        private static Dictionary<string, string> __aliasMap = new Dictionary<string, string>();

        /**
         * 注册接口 实现类; 
         * @param interfaceName 接口名称
         * @param clazz 实现类
         * @return 是否注册成功;
         * 
         */
        public static void registerClass<T>(string aliasName="") where T : new()
        {
            Type clazz = typeof(T);
            if (string.IsNullOrEmpty(aliasName))
            {
                aliasName = clazz.Name;
            }
            registerClass(clazz, aliasName);
        }

        public static void registerClass(Type clazz, string aliasName="")
        {
            string fullClassName = clazz.FullName;
            __aliasMap[fullClassName] = aliasName;
            __classMap[aliasName] = clazz;
        }

        public static void unregisterClass(string aliasName)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                DebugX.LogWarning("aliasName is empty:" + aliasName);
                return;
            }
            if (__classMap.ContainsKey(aliasName))
            {
                __classMap.Remove(aliasName);
            }
        }


        /// <summary>
        /// 注册一个实例与名称对应的关系;
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="instance"></param>
        public static void registerSingleton(string aliasName, object instance)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                DebugX.LogWarning("aliasName is empty:" + aliasName);
                return;
            }

            if (instance == null)
            {
                uniqueInstanceMap.Remove(aliasName);
                return;
            }
            Type clazz = instance.GetType();
            string fullClassName = clazz.FullName;
            __classMap[aliasName] = clazz;

            uniqueMap[aliasName] = clazz;
            uniqueMap[fullClassName] = clazz;

            __aliasMap[fullClassName] = aliasName;
            __aliasMap[aliasName] = aliasName;
            uniqueInstanceMap[aliasName] = instance;
        }

        public static void registerSingleton<T>(string aliasName) where T : new()
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                DebugX.LogWarning("aliasName is empty:" + aliasName);
                return;
            }

            Type clazz = typeof(T);

            string fullClassName = clazz.FullName;
            __classMap[aliasName] = clazz;

            __aliasMap[fullClassName] = aliasName;
            __aliasMap[aliasName] = aliasName;

            uniqueMap[aliasName] = clazz;
            uniqueMap[fullClassName] = clazz;
        }


        /// <summary>
        /// 取得别名;
        /// </summary>
        /// <returns>
        /// 别名,null为没有注册过别名
        /// </returns>
        /// <param name='fullName'>
        /// Full name.
        /// </param>
        public static string getAliasName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                DebugX.LogWarning("aliasName is empty:" + fullName);
                return null;
            }
            string aliasName;

            if (__aliasMap.TryGetValue(fullName, out aliasName) == true)
            {
                return aliasName;
            }

            return null;
        }


        /**
         * 取得一个实现接口的实例
         * @param interfaceName
         * @return 
         * 
         */
        public static object __getOneInstance(string aliasName)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                DebugX.LogWarning("aliasName is empty:" + aliasName);
                return null;
            }
            object target = null;
            if (uniqueInstanceMap.TryGetValue(aliasName, out target))
            {
                return target;
            }

            Type c;
            if (uniqueMap.TryGetValue(aliasName, out c) == true)
            {
                object ins = Activator.CreateInstance(c);
                uniqueInstanceMap[aliasName] = ins;
                uniqueInstanceMap[c.FullName] = ins;
                return ins;
            }

            c = getClass(aliasName);

            if (c == null)
            {
                return null;
            }

            return Activator.CreateInstance(c);
        }

        /**
         * 取得一个实现接口的类 
         * @param interfaceName
         * @return 
         * 
         */
        public static Type getClass(string aliasName)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                DebugX.LogWarning("aliasName is empty:"+aliasName);
                return null;
            }
            Type c;
            if (__classMap.TryGetValue(aliasName, out c) == false)
            {
                c = Type.GetType(aliasName);
            }
            return c;
        }


        public static bool isInUnique(string aliasName)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                DebugX.LogWarning("aliasName is empty:" + aliasName);
                return false;
            }
            if (uniqueInstanceMap.ContainsKey(aliasName))
            {
                return true;
            }

            return uniqueMap.ContainsKey(aliasName);
        }

        /**
         * 取得实现接口的单例,必须含有 getInstance的静态方法;
         * @param interfaceName 接口名称;
         * @return 返回实现的单例
         * 
         */
        public static object getInstance(string aliasName)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                DebugX.LogWarning("aliasName is empty:" + aliasName);
                return null;
            }

            object ins;
            if (uniqueInstanceMap.TryGetValue(aliasName, out ins) == false)
            {
                Type c;
                if (uniqueMap.TryGetValue(aliasName, out c) == false)
                {
                    c = getClass(aliasName);
                }

                if (c == null)
                {
                    return null;
                }
                uniqueInstanceMap[aliasName] = ins = Activator.CreateInstance(c);
            }
            return ins;
        }


        public static T getInstance<T>() where T : new()
        {
            Type clazz = typeof(T);
            string fullName = clazz.FullName;

            T instance = (T)Singleton.getInstance(fullName);

            if (instance == null)
            {
                instance = (T)Activator.CreateInstance(clazz);
                uniqueInstanceMap[fullName] = instance;
                uniqueInstanceMap[clazz.Name] = instance;
            }
            return instance;
        }


    }
}
