using System;
using System.Collections.Generic;
using System.Xml;

namespace foundation
{
    public class ConfigurationUtil
    {
        /// <summary>
        /// PREFIX 包 locaname。
        /// 如果需要修改xml，可在此处修改
        /// </summary>
        public static readonly string PREFIXS = "prefixes";

        public static readonly string SERVICES = "services";
        public static readonly string RESOURCES = "resources";
        public static readonly string WEBS = "webs";


        private Dictionary<string, Type> sectionRegs;
        public Dictionary<string, Dictionary<string, AbstractSection>> configuration;
        private bool _isConfigExist = false;
        public bool querysEnabled = true;
        public string version;

        public ConfigurationUtil()
        {
            sectionRegs = new Dictionary<string, Type>();

            configuration = new Dictionary<string, Dictionary<string, AbstractSection>>();

            registSectionParser(PREFIXS, typeof(AbstractSection));
            registSectionParser(SERVICES, typeof(ServiceSection));
            registSectionParser(RESOURCES, typeof(ResourceSection));
            registSectionParser(WEBS, typeof(ResourceSection));
        }

        public void registSectionParser(string sectionName, Type parserClass)
        {

            if (sectionRegs.ContainsKey(sectionName))
            {
                sectionRegs.Remove(sectionName);
            }

            sectionRegs.Add(sectionName, parserClass);
        }

        private static ConfigurationUtil instance = new ConfigurationUtil();

        public bool isConfigExist
        {
            get { return _isConfigExist; }
        }

        private List<AbstractSection> getSectionsByXML(XmlNodeList list, Type cls)
        {
            List<AbstractSection> result = new List<AbstractSection>();

            AbstractSection se;
            foreach (XmlNode ele in list)
            {

                if (ele is XmlComment)
                {
                    continue;
                }

                se = (AbstractSection) Activator.CreateInstance(cls);
                se.config = this;
                se.bindXML(ele);

                result.Add(se);
            }

            return result;
        }

        static public void initConfigXML(XmlNodeList xml, string version = "1.0")
        {
            instance._initConfigXML(xml, version);
        }

        private void _initConfigXML(XmlNodeList xml, string version = "1.0")
        {
            this.version = version;

            string localName;
            foreach (XmlNode element in xml)
            {
                localName = element.LocalName;

                List<AbstractSection> list;

                Type cls = null;
                if (sectionRegs.TryGetValue(localName, out cls))
                {
                    list = getSectionsByXML(element.ChildNodes, cls);
                }
                else
                {
                    //RFTraceError('不存在命令解析');
                    continue;
                }

                Dictionary<string, AbstractSection> dic = null;
                if (configuration.TryGetValue(localName, out dic) == false)
                {
                    dic = new Dictionary<string, AbstractSection>();
                    configuration.Add(localName, dic);
                }

                foreach (AbstractSection item in list)
                {
                    if (dic.ContainsKey(item.name))
                    {
                        //RFTraceError('xml node ' + item.name + ' exist');
                        continue;
                    }
                    dic.Add(item.name, item);
                }
            }

            _isConfigExist = true;
        }



        public static string getPrefix(string value)
        {
            return instance.getItemURL(PREFIXS, value);
        }

        public static string getResource(string value)
        {
            return instance.getItemURL(RESOURCES, value);
        }

        public static string getService(string value)
        {
            return instance.getItemURL(SERVICES, value);
        }
        public static string getWeb(string value)
        {
            return instance.getItemURL(WEBS, value);
        }

        public static AbstractSection getPrefixSection(string value)
        {
            return instance.getSection(PREFIXS, value);
        }

        public static AbstractSection getResourceSection(string value)
        {
            return instance.getSection(RESOURCES, value);
        }

        public static AbstractSection getServiceSection(string value)
        {
            return instance.getSection(SERVICES, value);
        }
        public static AbstractSection getWebSection(string value)
        {
            return instance.getSection(WEBS, value);
        }

        public AbstractSection getSection(string nodeName, string attributeName)
        {
            Dictionary<string, AbstractSection> dic = null;
            if (configuration.TryGetValue(nodeName, out dic) == false)
            {
                return null;
            }

            AbstractSection se = null;
            if (dic.TryGetValue(attributeName, out se) == false)
            {
                return null;
            }

            return se;
        }

        public string getItemURL(string nodeName, string attributeName)
        {
            string v = "";
            if (_isConfigExist == false)
            {
                return v;
            }

            Dictionary<string, AbstractSection> dic = null;
            if (configuration.TryGetValue(nodeName, out dic) == false)
            {
                return v;
            }

            AbstractSection se = null;
            if (dic.TryGetValue(attributeName, out se) == false)
            {
                return v;
            }

            if (se != null)
            {
                v = se.value;
            }

            return v;
        }


        public static void setPrefixValue(string name, string value)
        {
            if (!instance._isConfigExist)
            {
                return;
            }
            Dictionary<string, AbstractSection> sections = null;

            if (instance.configuration.TryGetValue(PREFIXS, out sections))
            {
                AbstractSection prefixSection;
                if (sections.TryGetValue(name, out prefixSection))
                {
                    prefixSection.value = value;
                }
            }
        }
    }
}
