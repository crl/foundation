using System.Xml;

namespace foundation
{
    public class ResourceSection : AbstractSection
    {
        /// <summary>
        /// 是否加入版本后缀
        /// </summary>
        public bool v = false;

        /// <summary>
        /// 加在配置value的前缀 
        /// </summary>
        protected string prefix;

        /// <summary>
        /// 是否值被初始化过; 
        /// </summary>
        protected bool _inited = false;
        public ResourceSection()
        {
        }
        override public void bindXML(XmlNode xml)
        {
            base.bindXML(xml);
            prefix = getAttributeValue(xml.Attributes, "prefix");
            v = getAttributeValue(xml.Attributes,"v") != "";
        }

        override public string value {

            get {

                if (_inited) {
                    return _value;
                }

                _inited = true;

                if (prefix != "") {
                    AbstractSection prefixSecion = _conf.getSection(ConfigurationUtil.PREFIXS,prefix);
                    if (prefixSecion !=null) {
                        _value = prefixSecion.value + _value;
                    }
                }

                if (!_conf.querysEnabled) {
                    return _value;
                }


                if (string.IsNullOrEmpty(_hash)==false) {
                    _value = _value + "?h=" + _hash;
                }

                if (v) {
                    _value = _value + getSpcode(_value) + "v=" + _conf.version;
                }
                return _value;
            }
        }
    }
}
