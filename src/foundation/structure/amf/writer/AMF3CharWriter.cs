using System;

namespace foundation
{
	class AMF3CharWriter : IAMFWriter
	{
		public AMF3CharWriter()
		{
		}
		#region IAMFWriter Members

		public bool IsPrimitive{ get{ return true; } }

		public void WriteData(AMFWriter writer, object data)
		{
			writer.WriteAMF3String( new String( (char)data, 1)  );
		}

		#endregion
	}
}
