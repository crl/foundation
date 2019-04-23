using System;

namespace foundation
{
	class AMF3EnumWriter : IAMFWriter
	{
		public AMF3EnumWriter()
		{
		}
		#region IAMFWriter Members

		public bool IsPrimitive{ get{ return true; } }

		public void WriteData(AMFWriter writer, object data)
		{
			int value = Convert.ToInt32(data);
			writer.WriteAMF3Int(value);
		}

		#endregion
	}
}
