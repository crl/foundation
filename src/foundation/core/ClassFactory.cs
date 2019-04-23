using System;
using System.Collections.Generic;
using System.Reflection;

namespace foundation
{
    public class ClassFactory<T> : IFactory
    {
        //类初始化时自动初始化的额外属性	;
        public Dictionary<string, object> properties = null;
        public virtual object newInstance()
        {
            Type clazz = typeof (T);
            T instance = (T)Activator.CreateInstance(clazz);

            if (properties != null)
            {
                FieldInfo fieldInfo;
                foreach (string key  in properties.Keys)
                {
                    fieldInfo = clazz.GetField(key);
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(instance, properties[key]);
                    }
                }
            }

            return instance;
        }
    }

    public interface ISizeFactory
    {
        int itemWidth { get; }

        int itemHeight { get; }
    }


  
}
