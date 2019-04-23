using System;

namespace foundation
{
	class AMF3NumberReader : IAMFReader
	{
		public AMF3NumberReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return reader.ReadDouble();
            //AMF3 undefined = double.NaN
		}

		#endregion
	}
}
