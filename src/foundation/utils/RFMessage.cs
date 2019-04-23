using System.Collections;
using System.Collections.Generic;

namespace foundation
{
    public class RFMessage
    {
        public static Dictionary<string, string> messageDict = new Dictionary<string, string>();

        public static string getMessage(int code, params object[] args)
        {
            return getMessage(code.ToString(), args);
        }
        public static string getMessage(string code, params object[] args)
        {
            string value = null;
            if (!messageDict.TryGetValue(code, out value))
            {
                value = code;
            }

            if (args.Length > 0)
            {
                value = StringUtil.substitute(value, args);
            }
            return value;
        }

        public static string getConfig(int code,string def=null)
        {
            string value = null;
            if (messageDict.TryGetValue(code.ToString(), out value)==false)
            {
                value = def;
            }
            return value;
        }
        public static string getConfig(string code, string def = null)
        {
            string value = null;
            if (messageDict.TryGetValue(code, out value)==false)
            {
                value = def;
            }
            return value;
        }

        public static void decode(IDictionary o)
        {
            if (o == null)
            {
                return;
            }
            foreach (string key in o.Keys)
            {
                if (messageDict.ContainsKey(key) == false)
                {
                    messageDict.Add(key, o[key].ToString());
                }
            }
        }
    }
}