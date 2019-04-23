namespace foundation
{
    class AMF3BooleanTrueReader : IAMFReader
	{
		public AMF3BooleanTrueReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return true;
		}

		#endregion
	}
}
