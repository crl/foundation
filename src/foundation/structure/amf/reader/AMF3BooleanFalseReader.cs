namespace foundation
{
    class AMF3BooleanFalseReader : IAMFReader
	{
		public AMF3BooleanFalseReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return false;
		}

		#endregion
	}
}
