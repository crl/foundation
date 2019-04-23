namespace foundation
{
    class AMF3NullReader : IAMFReader
	{
		public AMF3NullReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return null;
		}

		#endregion
	}
}
