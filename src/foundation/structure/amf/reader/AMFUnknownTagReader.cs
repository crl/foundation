using System;

namespace foundation
{
	class AMFUnknownTagReader : IAMFReader
	{
		public AMFUnknownTagReader()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
            return null;
		}

		#endregion
	}

 
    class MovieclipMarker : IAMFReader
    {
        public MovieclipMarker()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
            throw new Exception("not imp");
		}

		#endregion
    }

    class UnsupportedMarker : IAMFReader
    {
        public UnsupportedMarker()
		{
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			throw new Exception("not imp");
		}

		#endregion
    }
}
