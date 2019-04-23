using System;
using System.Xml;


namespace foundation
{
	class AMF3XmlReader : IAMFReader
	{
		public AMF3XmlReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return reader.ReadAMF3XmlDocument();
		}

		#endregion
	}
}
