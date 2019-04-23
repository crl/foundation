namespace foundation
{
    public interface IAMFSerializer
    { 
        void AMFDeserialize(IDataInput input);

        void AMFSerialize(IDataOutput output);
    }
}