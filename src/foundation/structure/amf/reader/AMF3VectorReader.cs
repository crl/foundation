using System;

namespace foundation
{
    class AMF3IntVectorReader : IAMFReader
    {
        public AMF3IntVectorReader()
		{
		}

        #region IAMFReader Members

        public object ReadData(AMFReader reader)
        {
            return reader.ReadAMF3IntVector();
        }

        #endregion
    }

    class AMF3UIntVectorReader : IAMFReader
    {
        public AMF3UIntVectorReader()
        {
        }

        #region IAMFReader Members

        public object ReadData(AMFReader reader)
        {
            return reader.ReadAMF3UIntVector();
        }

        #endregion
    }

    class AMF3DoubleVectorReader : IAMFReader
    {
        public AMF3DoubleVectorReader()
        {
        }

        #region IAMFReader Members

        public object ReadData(AMFReader reader)
        {
            return reader.ReadAMF3DoubleVector();
        }

        #endregion
    }

    class AMF3ObjectVectorReader : IAMFReader
    {
        public AMF3ObjectVectorReader()
        {
        }

        #region IAMFReader Members

        public object ReadData(AMFReader reader)
        {
            return reader.ReadAMF3ObjectVector();
        }

        #endregion
    }
}
