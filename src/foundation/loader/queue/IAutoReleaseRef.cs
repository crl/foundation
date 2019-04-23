namespace foundation
{
    public interface IAutoReleaseRef
    {
        int release();
		int retain();
		int retainCount
        {
            get;
        }
		void __dispose();
    }
}
