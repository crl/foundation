namespace foundation
{
    class DataOutput : IDataOutput
	{
		private AMFWriter _amfWriter;
        private ObjectEncoding _objectEncoding;

		public DataOutput(AMFWriter amfWriter)
		{
			_amfWriter = amfWriter;
            _objectEncoding = ObjectEncoding.AMF3;
		}

        public ObjectEncoding ObjectEncoding
        {
            get { return _objectEncoding; }
            set { _objectEncoding = value; }
        }

		#region IDataOutput Members

		public void WriteBoolean(bool value)
		{
			_amfWriter.WriteBoolean(value);
		}

		public void WriteByte(byte value)
		{
			_amfWriter.WriteByte(value);
		}
	
		public void WriteBytes(byte[] bytes, int offset, int length)
		{
			for(int i = offset; i < offset + length; i++)
				_amfWriter.WriteByte( bytes[i] );
		}
		
		public void WriteDouble(double value)
		{
			_amfWriter.WriteDouble(value);
		}
		
		public void WriteFloat(float value)
		{
			_amfWriter.WriteFloat(value);
		}
		
		public void WriteInt(int value)
		{
			_amfWriter.WriteInt32(value);
		}

        public void WriteObject(object value)
        {
            if (_objectEncoding == ObjectEncoding.AMF3)
            {
                _amfWriter.WriteAMF3Data(value);
            }
        }
		
		public void WriteShort(short value)
		{
			_amfWriter.WriteShort(value);
		}
		
		public void WriteUnsignedInt(uint value)
		{
			_amfWriter.WriteInt32((int)value);
		}
		
		public void WriteUTF(string value)
		{
			_amfWriter.WriteUTF(value);
		}
		
		public void WriteUTFBytes(string value)
		{
			_amfWriter.WriteUTFBytes(value);
		}

		#endregion
	}
}
