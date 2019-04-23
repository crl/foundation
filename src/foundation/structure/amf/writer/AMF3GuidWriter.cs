using System;

namespace foundation
{
	class AMF3GuidWriter : IAMFWriter
	{
		public AMF3GuidWriter()
		{
		}

		#region IAMFWriter Members

		public bool IsPrimitive{ get{ return true; } }

		public void WriteData(AMFWriter writer, object data)
		{
			writer.WriteAMF3String( ((Guid)data).ToString() );
		}

		#endregion
	}
}
