namespace foundation
{
    public interface IRecycleFactory
    {
        IRecycleable getRecycle();
		
		void recycle(IRecycleable value);

        void cleanup();
    }


    public interface IRecycleable
    {
        bool recycled { get; }
        void recycle();

        void awaken();

        IRecycleFactory factory
        {
            get; set; }
    }
}