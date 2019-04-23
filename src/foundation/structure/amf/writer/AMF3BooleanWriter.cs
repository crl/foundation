namespace foundation
{
    class AMF3BooleanWriter : IAMFWriter
	{
		public AMF3BooleanWriter()
		{
		}
		#region IAMFWriter Members

		public bool IsPrimitive{ get{ return true; } }

		public void WriteData(AMFWriter writer, object data)
		{
			writer.WriteAMF3Bool((bool)data);
		}

		#endregion
	}
}
