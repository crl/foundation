namespace foundation
{
    class AMF3ObjectReader : IAMFReader
	{
		public AMF3ObjectReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
            return reader.ReadAMF3Object();
		}

		#endregion
	}
}
