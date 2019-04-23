namespace foundation
{
    class AMF3DateTimeReader : IAMFReader
	{
		public AMF3DateTimeReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return reader.ReadAMF3Date();
		}

		#endregion
	}
}
