using System.Xml;

namespace foundation
{
    public class AbstractSection : IXMLBinder
    {
        virtual public string value
        {
            get
            {
               return _value;
            }
            set { _value = value; }
        }

        public string extends
        {
            get { return _extends; }
        }

        protected string _name;
        protected string _value;
        protected string _hash;
        protected string _version;
        protected string _extends;

        protected ConfigurationUtil _conf;
        public AbstractSection()
        {
        }

        public string name
        {
            get
            {
                return _name;
            }
        }

        public string version
        {
            get { return _version; }
        }

        public static string getSpcode(string value)
        {
            string sp = "?";
            if (value.IndexOf(sp) != -1)
            {
                sp = "&";
            }
            return sp;
        }


        public ConfigurationUtil config
        {
            set
            {
                this._conf = value;
            }
        }

        virtual public void bindXML(XmlNode xml)
        {
            _name = getAttributeValue(xml.Attributes, "name");
            _value = getAttributeValue(xml.Attributes, "value");

            if (string.IsNullOrEmpty(_value))
            {
                _value = xml.InnerText;
            }

            _version = getAttributeValue(xml.Attributes, "version");
            _hash = getAttributeValue(xml.Attributes, "hash");
            _extends= getAttributeValue(xml.Attributes, "extends");
        }
        internal string getAttributeValue(XmlAttributeCollection attributes,string key)
        {
            XmlAttribute attribute=attributes[key];

            if (attribute != null)
            {
                return attribute.InnerText.Trim();
            }
            return "";
        }
    }
}
