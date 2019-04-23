using System;

namespace foundation
{
    class AMF3DateTimeWriter : IAMFWriter
	{
		public AMF3DateTimeWriter()
		{
		}
		#region IAMFWriter Members

		public bool IsPrimitive{ get{ return true; }}

		public void WriteData(AMFWriter writer, object data)
		{
			writer.WriteByte(AMF3TypeCode.DateTime);
			writer.WriteAMF3DateTime((DateTime)data);
		}

		#endregion
	}
}
