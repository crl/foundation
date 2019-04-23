namespace foundation
{
    public class AMF3DictionaryReader:IAMFReader
    {
        public object ReadData(AMFReader reader)
        {

            return reader.ReadDictionary();
        }
    }
}
