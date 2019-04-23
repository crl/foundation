namespace foundation
{
    public interface ILoaderFactory
    {
        RFLoader getLoader(AssetResource resource);
    }
}