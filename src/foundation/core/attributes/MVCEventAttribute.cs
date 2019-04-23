using System;
using System.Collections.Generic;
using System.Reflection;

namespace foundation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MVCEventAttribute:Attribute
    {
        private InjectEventType _injectEventType = InjectEventType.Show;

        public InjectEventType injectEventType
        {
            get { return _injectEventType; }
            private set { _injectEventType = value; }
        }

        public string[] eventList
        {
            get;
            private set;
        }
        public MVCEventAttribute(params string[] eventList)
        {
            this.eventList = eventList;
        }

        public MVCEventAttribute(InjectEventType injectEventType, params string[] eventList)
        {
            this.injectEventType = injectEventType;
            this.eventList = eventList;
        }

        public static Dictionary<InjectEventType, Dictionary<string, Action<EventX>>> CollectionEventInterests(
            IEventInterester instance,
            Dictionary<InjectEventType, Dictionary<string, Action<EventX>>> _eventInterests = null)
        {
            Type type = instance.GetType();
            if (_eventInterests == null)
            {
                _eventInterests = new Dictionary<InjectEventType, Dictionary<string, Action<EventX>>>();
            }
            Dictionary<string, Action<EventX>> dic = null;
            MethodInfo[] methods =
                type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MVCEventAttribute attr;
            Type attributeType = typeof(MVCEventAttribute);
            int len = methods.Length;
            for (int i = 0; i < len; i++)
            {
                MethodInfo info = methods[i];
                object[] attrs = info.GetCustomAttributes(attributeType, true);
                int alen = attrs.Length;
                for (int j = 0; j < alen; j++)
                {
                    attr = attrs[j] as MVCEventAttribute;

                    if (_eventInterests.TryGetValue(attr.injectEventType, out dic) == false)
                    {
                        dic = new Dictionary<string, Action<EventX>>();
                        _eventInterests.Add(attr.injectEventType, dic);
                    }
                    foreach (string eventType in attr.eventList)
                    {
                        if (dic.ContainsKey(eventType) == false)
                        {
                            dic.Add(eventType, (EventX e) =>
                            {
                                try
                                {
                                    info.Invoke(instance, new[] { e });
                                }
                                catch (Exception exception)
                                {
                                    if (exception.InnerException != null)
                                    {
                                        DebugX.LogError(exception.InnerException.ToString());
                                    }
                                    //DebugX.LogError("class:"+ type.Name+ "MethodInfo:"+ info.Name);
                                }
                                
                            });
                        }
                        else
                        {
                            DebugX.Log("MVCEventAttribute inject:{0} type:{1} not exist event:{2}", type.FullName,
                                attr.injectEventType, eventType);
                        }
                    }

                }
            }
            return _eventInterests;
        }
    }

    public enum InjectEventType
    {
        //show or hide panel regist facadeDispatchEvent
        Show,
        //always regist facadeDispatchEvent
        Always,
        //proxy event
        Model,
    }
}