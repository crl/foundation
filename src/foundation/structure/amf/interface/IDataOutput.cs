namespace foundation
{
    public interface IDataOutput
	{
		void WriteBoolean(bool value);

		void WriteByte(byte value);

		void WriteBytes(byte[] bytes, int offset, int length);

		void WriteDouble(double value);
	
		void WriteFloat(float value);

		void WriteInt(int value);

		void WriteObject(object value);
		
		void WriteShort(short value);
	
		void WriteUnsignedInt(uint value);

		void WriteUTF(string value);

		void WriteUTFBytes(string value);
	}
}
