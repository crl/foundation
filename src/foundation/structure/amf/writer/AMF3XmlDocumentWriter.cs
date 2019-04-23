using System.Xml;

namespace foundation
{
    class AMF3XmlDocumentWriter : IAMFWriter
	{
		public AMF3XmlDocumentWriter()
		{
		}
		#region IAMFWriter Members

		public bool IsPrimitive{ get{return false;} }

        public void WriteData(AMFWriter writer, object data)
		{
			writer.WriteAMF3XmlDocument(data as XmlDocument);
		}
		#endregion
	}
}
