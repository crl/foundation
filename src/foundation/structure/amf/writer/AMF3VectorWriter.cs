using System.Collections;
using System.Collections.Generic;

namespace foundation
{
    internal class AMF3ObjectVectorWriter : IAMFWriter
    {
        #region IAMFWriter Members

        public bool IsPrimitive
        {
            get { return false; }
        }

        public void WriteData(AMFWriter writer, object data)
        {
            IList list = data as IList;
            if (list != null)
            {
                writer.WriteByte(AMF3TypeCode.ObjectVector);
                writer.WriteAMF3ObjectVector(list);
            }
        }

        #endregion
    }
}
