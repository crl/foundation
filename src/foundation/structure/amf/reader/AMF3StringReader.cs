using System;

namespace foundation
{
	class AMF3StringReader : IAMFReader
	{
		public AMF3StringReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			return reader.ReadAMF3String();
		}

		#endregion
	}
}
