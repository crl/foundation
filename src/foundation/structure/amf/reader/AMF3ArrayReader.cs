namespace foundation
{
    class AMF3ArrayReader : IAMFReader
	{
		public AMF3ArrayReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return reader.ReadAMF3Array();
		}

		#endregion
	}
}
