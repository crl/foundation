namespace foundation
{
    public class HashSizeFile: ISortWeight
    {
        public string uri;
        public int size;
        public string hash;

        public HashSizeFile(string uri)
        {
            this.uri = uri;
        }

        public long getSortWeight()
        {
            return size;
        }
    }
}