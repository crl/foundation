namespace foundation
{
    class AMF3CacheableObjectWriter : IAMFWriter
    {
        public AMF3CacheableObjectWriter()
        {
        }

        #region IAMFWriter Members

        public bool IsPrimitive { get { return true; } }

        public void WriteData(AMFWriter writer, object data)
        {
            writer.WriteAMF3Data(data);
        }

        #endregion
    }
}
