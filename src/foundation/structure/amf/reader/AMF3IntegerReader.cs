namespace foundation
{
    class AMF3IntegerReader : IAMFReader
	{
		public AMF3IntegerReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return reader.ReadAMF3Int();
		}

		#endregion
	}
}
