namespace foundation
{

    public interface IExternalizable
	{

		void ReadExternal(IDataInput input);

		void WriteExternal(IDataOutput output);
	}
}
