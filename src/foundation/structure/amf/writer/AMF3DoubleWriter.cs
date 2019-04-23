using System;

namespace foundation
{
	class AMF3DoubleWriter : IAMFWriter
	{
		public AMF3DoubleWriter()
		{
		}

		#region IAMFWriter Members

		public bool IsPrimitive{ get{ return true; } }

		public void WriteData(AMFWriter writer, object data)
		{
			double value = Convert.ToDouble(data);
			writer.WriteAMF3Double(value);
		}

		#endregion
	}
}
