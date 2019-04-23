using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class Executer
    {
        private static Dictionary<string, IParser> parserMapping=new Dictionary<string, IParser>();
        public static void execute(string type,object o=null,params object[] args)
        {
            IParser parser = null;
            if (parserMapping.TryGetValue(type, out parser) == false)
            {
                DebugX.Log("Executer type:{0} 解析不存在",type);
                return;
            }

            parser.parse(o,args);
        }

        public static void register<T>(string type) where  T:IParser
        {
            IParser parser = null;
            if (parserMapping.TryGetValue(type, out parser) == false)
            {
                parser= Activator.CreateInstance<T>();
                parserMapping.Add(type,parser);
            }
        }
    }
}
