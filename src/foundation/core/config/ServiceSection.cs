using System;
using System.Collections.Generic;
using System.Xml;

namespace foundation
{
    public class ServiceSection : ResourceSection
    {
        /// <summary>
        /// 是否添加随机参数; 
        /// </summary>
        protected bool r = false;
        override public void bindXML(XmlNode xml)
        {
            base.bindXML(xml);
            r = getAttributeValue(xml.Attributes,"r") != "";

            /*CONFIG::debugging
			{
				//如果不是air;
				//trace(Capabilities.playerType);
				if(Capabilities.playerType !="Desktop")r=true;
			}*/
        }

        override public string value
        {
            get
            {
                if (_inited)
                {
                    return _value;
                }
                _value = base.value;

                if (r && _conf.querysEnabled)
                {
                    _value = _value + getSpcode(_value) + "r=" + DateTime.Now.Millisecond;
                }
                return _value;
            }
        }
    }
}
