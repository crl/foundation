namespace foundation
{

    class AMF3StringWriter : IAMFWriter
	{
		public AMF3StringWriter()
		{
		}
		#region IAMFWriter Members

		public bool IsPrimitive{ get{ return true; } }

		public void WriteData(AMFWriter writer, object data)
		{
			writer.WriteAMF3String(data as string);
		}

		#endregion
	}
}
