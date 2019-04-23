namespace foundation
{
    class DataInput : IDataInput
	{
		private AMFReader _amfReader;
        private ObjectEncoding _objectEncoding;

		public DataInput(AMFReader amfReader)
		{
			_amfReader = amfReader;
            _objectEncoding = ObjectEncoding.AMF3;
		}

        public ObjectEncoding ObjectEncoding
        {
            get { return _objectEncoding; }
            set { _objectEncoding = value; }
        }

		#region IDataInput Members

		public bool ReadBoolean()
		{
			return _amfReader.ReadBoolean();
		}
		public byte ReadByte()
		{
			return _amfReader.ReadByte();
		}

		public void ReadBytes(byte[] bytes, uint offset, uint length)
		{
			byte[] tmp = _amfReader.ReadBytes((int)length);
			for(int i = 0; i < tmp.Length; i++)
				bytes[i+offset] = tmp[i];
		}

		public double ReadDouble()
		{
			return _amfReader.ReadDouble();
		}

		public float ReadFloat()
		{
			return _amfReader.ReadFloat();
		}

		public int ReadInt()
		{
			return _amfReader.ReadInt32();
		}

		public object ReadObject()
		{
            object obj = null;
            if (_objectEncoding == ObjectEncoding.AMF0)
            {
                //obj = _amfReader.ReadData();
            }
            if (_objectEncoding == ObjectEncoding.AMF3)
            {
                obj = _amfReader.ReadAMF3Data();
            }
            return obj;
		}
	
		public short ReadShort()
		{
			return _amfReader.ReadInt16();
		}

		public byte ReadUnsignedByte()
		{
			return _amfReader.ReadByte();
		}
	
		public uint ReadUnsignedInt()
		{
			return (uint)_amfReader.ReadInt32();
		}
		
		public ushort ReadUnsignedShort()
		{
			return _amfReader.ReadUInt16();
		}
		
		public string ReadUTF()
		{
			return _amfReader.ReadString();
		}
		
		public string ReadUTFBytes(uint length)
		{
			return _amfReader.ReadUTF((int)length);
		}

		#endregion
	}
}
