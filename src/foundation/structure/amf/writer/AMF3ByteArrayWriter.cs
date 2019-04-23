namespace foundation
{
    class AMF3ByteArrayWriter : IAMFWriter
	{
		public AMF3ByteArrayWriter()
		{
		}
		#region IAMFWriter Members

		public bool IsPrimitive{ get{return false;} }

		public void WriteData(AMFWriter writer, object data)
		{
			if(data is byte[])
				data = new ByteArray(data as byte[]);

			if(data is ByteArray)
			{
				writer.WriteByteArray(data as ByteArray);
			}
		}

		#endregion
	}
}
