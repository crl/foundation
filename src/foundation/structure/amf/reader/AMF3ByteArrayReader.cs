namespace foundation
{

    class AMF3ByteArrayReader : IAMFReader
	{
		public AMF3ByteArrayReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return reader.ReadAMF3ByteArray();
		}

		#endregion
	}
}
