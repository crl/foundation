namespace foundation
{
    public interface IDataInput
	{

		bool ReadBoolean();

		byte ReadByte();

		void ReadBytes(byte[] bytes, uint offset, uint length);

		double ReadDouble();
		
		float ReadFloat();
		
		int ReadInt();
		
		object ReadObject();
		
		short ReadShort();
		
		byte ReadUnsignedByte();
		
		uint ReadUnsignedInt();
		
		ushort ReadUnsignedShort();
		
		string ReadUTF();
		
		string ReadUTFBytes(uint length);
	}
}
