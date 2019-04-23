using System;
using System.IO;
using System.ComponentModel;
using Ionic.Zlib;

namespace foundation
{
    public class ByteArrayConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(byte[]))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(byte[]))
            {
                return ((ByteArray)value).MemoryStream.ToArray();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public static class CompressionAlgorithm
    {
        public const string Deflate = "deflate";
        public const string Zlib = "zlib";
    }

    public enum ObjectEncoding
    {
        AMF0 = 0,
        AMF3 = 3
    }

    [TypeConverter(typeof(ByteArrayConverter))]
    public class ByteArray : IDataInput, IDataOutput
    {
        private MemoryStream _memoryStream;
        private DataOutput _dataOutput;
        private DataInput _dataInput;
        private ObjectEncoding _objectEncoding;

        public ByteArray()
        {
            _memoryStream = new MemoryStream();
            AMFReader amfReader = new AMFReader(_memoryStream);
            AMFWriter amfWriter = new AMFWriter(_memoryStream);
            _dataOutput = new DataOutput(amfWriter);
            _dataInput = new DataInput(amfReader);
            _objectEncoding = ObjectEncoding.AMF3;
        }

        public ByteArray(MemoryStream ms)
        {
            _memoryStream = ms;
            AMFReader amfReader = new AMFReader(_memoryStream);
            AMFWriter amfWriter = new AMFWriter(_memoryStream);
            _dataOutput = new DataOutput(amfWriter);
            _dataInput = new DataInput(amfReader);
            _objectEncoding = ObjectEncoding.AMF3;
        }

        public ByteArray(byte[] buffer)
        {
            _memoryStream = new MemoryStream();
            _memoryStream.Write(buffer, 0, buffer.Length);
            _memoryStream.Position = 0;
            AMFReader amfReader = new AMFReader(_memoryStream);
            AMFWriter amfWriter = new AMFWriter(_memoryStream);
            _dataOutput = new DataOutput(amfWriter);
            _dataInput = new DataInput(amfReader);
            _objectEncoding = ObjectEncoding.AMF3;
        }

        public uint Length
        {
            get
            {
                return (uint)_memoryStream.Length;
            }
        }

        public uint Position
        {
            get { return (uint)_memoryStream.Position; }
            set { _memoryStream.Position = value; }
        }

        public uint BytesAvailable
        {
            get { return Length - Position; }
        }

        public ObjectEncoding ObjectEncoding
        {
            get { return _objectEncoding; }
            set
            {
                _objectEncoding = value;
                _dataInput.ObjectEncoding = value;
                _dataOutput.ObjectEncoding = value;
            }
        }

        /// <summary>
        /// 缓冲区大小的列表(含空的字节数组,速度相当快)
        /// </summary>
        /// <returns></returns>
        public byte[] GetBuffer()
        {
            return _memoryStream.GetBuffer();
        }

        /// <summary>
        /// 有byte的列表
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return _memoryStream.ToArray();
        }

        internal MemoryStream MemoryStream { get { return _memoryStream; } }

        public MemoryStream UnSafeMemoryStream
        {
            get { return _memoryStream; }
        }

        #region IDataInput Members

        public bool ReadBoolean()
        {
            return _dataInput.ReadBoolean();
        }

        public byte ReadByte()
        {
            return _dataInput.ReadByte();
        }

        public void ReadBytes(byte[] bytes, uint offset, uint length)
        {
            _dataInput.ReadBytes(bytes, offset, length);
        }

        public void ReadBytes(ByteArray bytes, uint offset, uint length)
        {
            uint tmp = bytes.Position;
            int count = (int)(length != 0 ? length : BytesAvailable);
            for (int i = 0; i < count; i++)
            {
                bytes._memoryStream.Position = i + offset;
                bytes._memoryStream.WriteByte(ReadByte());
            }
            bytes.Position = tmp;
        }

        public void ReadBytes(ByteArray bytes)
        {
            ReadBytes(bytes, 0, 0);
        }

        public double ReadDouble()
        {
            return _dataInput.ReadDouble();
        }

        public float ReadFloat()
        {
            return _dataInput.ReadFloat();
        }

        public int ReadInt()
        {
            return _dataInput.ReadInt();
        }

        public object ReadObject()
        {
            return _dataInput.ReadObject();
        }

        public short ReadShort()
        {
            return _dataInput.ReadShort();
        }

        public byte ReadUnsignedByte()
        {
            return _dataInput.ReadUnsignedByte();
        }

        public uint ReadUnsignedInt()
        {
            return _dataInput.ReadUnsignedInt();
        }

        public ushort ReadUnsignedShort()
        {
            return _dataInput.ReadUnsignedShort();
        }

        public string ReadUTF()
        {
            return _dataInput.ReadUTF();
        }

        public string ReadUTFBytes(uint length)
        {
            return _dataInput.ReadUTFBytes(length);
        }

        #endregion

        #region IDataOutput Members

        public void WriteBoolean(bool value)
        {
            _dataOutput.WriteBoolean(value);
        }

        public void WriteByte(byte value)
        {
            _dataOutput.WriteByte(value);
        }

        public void WriteBytes(byte[] bytes, int offset, int length)
        {
            _dataOutput.WriteBytes(bytes, offset, length);
        }

        public void WriteDouble(double value)
        {
            _dataOutput.WriteDouble(value);
        }

        public void WriteFloat(float value)
        {
            _dataOutput.WriteFloat(value);
        }

        public void WriteInt(int value)
        {
            _dataOutput.WriteInt(value);
        }

        public void WriteObject(object value)
        {
            _dataOutput.WriteObject(value);
        }

        public void WriteShort(short value)
        {
            _dataOutput.WriteShort(value);
        }

        public void WriteUnsignedInt(uint value)
        {
            _dataOutput.WriteUnsignedInt(value);
        }

        public void WriteUTF(string value)
        {
            _dataOutput.WriteUTF(value);
        }

        public void WriteUTFBytes(string value)
        {
            _dataOutput.WriteUTFBytes(value);
        }

        #endregion

        public void Compress()
        {
            Compress(CompressionAlgorithm.Zlib);
        }

        public void Deflate()
        {
            Compress(CompressionAlgorithm.Deflate);
        }



        public void Compress(string algorithm)
        {


            if (algorithm == CompressionAlgorithm.Deflate)
            {
                byte[] buffer = _memoryStream.ToArray();
                MemoryStream ms = new MemoryStream();
                DeflateStream deflateStream = new DeflateStream(ms, CompressionMode.Compress, true);
                deflateStream.Write(buffer, 0, buffer.Length);
                deflateStream.Close();
                _memoryStream.Close();
                _memoryStream = ms;
                AMFReader amfReader = new AMFReader(_memoryStream);
                AMFWriter amfWriter = new AMFWriter(_memoryStream);
                _dataOutput = new DataOutput(amfWriter);
                _dataInput = new DataInput(amfReader);
            }
            if (algorithm == CompressionAlgorithm.Zlib)
            {
                byte[] buffer = _memoryStream.ToArray();
                MemoryStream ms = new MemoryStream();
                ZlibStream zlibStream = new ZlibStream(ms, CompressionMode.Compress, true);
                zlibStream.Write(buffer, 0, buffer.Length);
                zlibStream.Flush();
                zlibStream.Close();
                zlibStream.Dispose();
                _memoryStream.Close();
                _memoryStream = ms;
                AMFReader amfReader = new AMFReader(_memoryStream);
                AMFWriter amfWriter = new AMFWriter(_memoryStream);
                _dataOutput = new DataOutput(amfWriter);
                _dataInput = new DataInput(amfReader);
            }
        }

        public void Inflate()
        {
            Uncompress(CompressionAlgorithm.Deflate);
        }


        public void Uncompress()
        {
            Uncompress(CompressionAlgorithm.Zlib);
        }

        public void Uncompress(string algorithm)
        {
            if (algorithm == CompressionAlgorithm.Zlib)
            {
                Position = 0;
                ZlibStream deflateStream = new ZlibStream(_memoryStream, CompressionMode.Decompress, false);
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[1024];
                // Chop off the first two bytes
                //int b = _memoryStream.ReadByte();
                //b = _memoryStream.ReadByte();
                while (true)
                {
                    int readCount = deflateStream.Read(buffer, 0, buffer.Length);
                    if (readCount > 0)
                        ms.Write(buffer, 0, readCount);
                    else
                        break;
                }
                deflateStream.Close();
                _memoryStream.Close();
                _memoryStream.Dispose();
                _memoryStream = ms;
                _memoryStream.Position = 0;
            }
            if (algorithm == CompressionAlgorithm.Deflate)
            {
                Position = 0;
                DeflateStream deflateStream = new DeflateStream(_memoryStream, CompressionMode.Decompress, false);
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int readCount = deflateStream.Read(buffer, 0, buffer.Length);
                    if (readCount > 0)
                        ms.Write(buffer, 0, readCount);
                    else
                        break;
                }
                deflateStream.Close();
                _memoryStream.Close();
                _memoryStream.Dispose();
                _memoryStream = ms;
                _memoryStream.Position = 0;
            }
            AMFReader amfReader = new AMFReader(_memoryStream);
            AMFWriter amfWriter = new AMFWriter(_memoryStream);
            _dataOutput = new DataOutput(amfWriter);
            _dataInput = new DataInput(amfReader);
        }

        public override string ToString()
        {
            return System.Text.Encoding.Default.GetString(ToArray());
        }
    }
}
