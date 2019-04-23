using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using foundation;

namespace foundation
{
    public class AMFWriter : BinaryWriter
	{
        Dictionary<Object, int> _objectReferences;
        Dictionary<Object, int> _stringReferences;
        Dictionary<ClassDefinition, int> _classDefinitionReferences;
  
        Dictionary<Type, IAMFWriter> amfWriterTable = new Dictionary<Type, IAMFWriter>();
        private static Dictionary<string, ClassDefinition> classDefinitions;

        public AMFWriter(Stream stream) : base(stream)
		{
            AMF3IntWriter amf3IntWriter = new AMF3IntWriter();
            AMF3DoubleWriter amf3DoubleWriter = new AMF3DoubleWriter();
            amfWriterTable.Add(typeof(System.SByte), amf3IntWriter);
            amfWriterTable.Add(typeof(System.Byte), amf3IntWriter);
            amfWriterTable.Add(typeof(System.Int16), amf3IntWriter);
            amfWriterTable.Add(typeof(System.UInt16), amf3IntWriter);
            amfWriterTable.Add(typeof(System.Int32), amf3IntWriter);
            amfWriterTable.Add(typeof(System.UInt32), amf3IntWriter);
            amfWriterTable.Add(typeof(System.Int64), amf3DoubleWriter);
            amfWriterTable.Add(typeof(System.UInt64), amf3DoubleWriter);
            amfWriterTable.Add(typeof(System.Single), amf3DoubleWriter);
            amfWriterTable.Add(typeof(System.Double), amf3DoubleWriter);
            amfWriterTable.Add(typeof(System.Decimal), amf3DoubleWriter);

            amfWriterTable.Add(typeof(XmlDocument), new AMF3XmlDocumentWriter());

            amfWriterTable.Add(typeof(string), new AMF3StringWriter());
            amfWriterTable.Add(typeof(bool), new AMF3BooleanWriter());
            amfWriterTable.Add(typeof(Enum), new AMF3EnumWriter());
            amfWriterTable.Add(typeof(Char), new AMF3CharWriter());
            amfWriterTable.Add(typeof(DateTime), new AMF3DateTimeWriter());
            amfWriterTable.Add(typeof(Array), new AMF3ArrayWriter());
            amfWriterTable.Add(typeof(ASObject), new AMF3ASObjectWriter());
            amfWriterTable.Add(typeof(ByteArray), new AMF3ByteArrayWriter());
            amfWriterTable.Add(typeof (byte[]), new AMF3ByteArrayWriter());

            amfWriterTable.Add(typeof(List<>), new AMF3ObjectVectorWriter());

            _objectReferences = new Dictionary<Object, int>(5);
            _stringReferences = new Dictionary<Object, int>(5);

            _classDefinitionReferences = new Dictionary<ClassDefinition, int>();

            classDefinitions = new Dictionary<string, ClassDefinition>();
		}

        public void WriteDouble(double value)
		{
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
		}

        public void WriteFloat(float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);			
			WriteBigEndian(bytes);
		}
    
        public void WriteInt32(int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			WriteBigEndian(bytes);
		}

        public void WriteBoolean(bool value)
        {
            this.BaseStream.WriteByte(value ? ((byte)1) : ((byte)0));
        }

        public void WriteUTF(string value)
        {
            if (value==null)
            {
                value = "";
            }
            //Length - max 65536.
            int byteCount = Encoding.UTF8.GetByteCount(value);
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            this.WriteShort(byteCount);
            if (buffer.Length > 0)
                base.Write(buffer);
        }

        public void WriteShort(int value)
        {
            byte[] bytes = BitConverter.GetBytes((ushort)value);
            WriteBigEndian(bytes);
        }

        public void WriteUTFBytes(string value)
        {
            if (value == null)
            {
                value = "";
            }
            //Length - max 65536.
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            if (buffer.Length > 0)
                base.Write(buffer);
        }


		private void WriteBigEndian(byte[] bytes)
		{
			if( bytes == null )
				return;
			for(int i = bytes.Length-1; i >= 0; i--)
			{
				base.BaseStream.WriteByte( bytes[i] );
			}
		}

        public void WriteByte(byte value)
		{
			this.BaseStream.WriteByte(value);
		}

		public void WriteBytes(byte[] buffer)
		{
			for(int i = 0; buffer != null && i < buffer.Length; i++)
				this.BaseStream.WriteByte(buffer[i]);
		}

        public void WriteAMF3Data(object data)
		{
			if( data == null )
			{
				WriteAMF3Null();
				return;
			}
            Type type = data.GetType();
      
            IAMFWriter amfWriter = null;
            if (amfWriterTable.ContainsKey(type)){
                amfWriter = amfWriterTable[type] as IAMFWriter;
            }else if (ReflectionUtils.IsSubClass(type, ReflectionUtils.GenericIListType))
            {
                amfWriterTable.Add(type, new AMF3ObjectVectorWriter());
            }
			//Second try with basetype (Enums for example)
            if (amfWriter == null && type.BaseType != null && amfWriterTable.ContainsKey(type.BaseType)){
                amfWriter = amfWriterTable[type.BaseType] as IAMFWriter;
            }

			if( amfWriter == null )
			{
                if (!amfWriterTable.ContainsKey(type))
                {
                    amfWriter = new AMF3ObjectWriter();
                    amfWriterTable.Add(type, amfWriter);
                }
                else{
                    amfWriter = amfWriterTable[type] as IAMFWriter;
                }
				
			}

			if( amfWriter != null )
			{
				amfWriter.WriteData(this, data);
            }
            else
            {
                throw new NotImplementedException(type.FullName+"not has amf3writer!");
            }
		}

		public void WriteAMF3Null()
		{
			//Write the null code (0x1) to the output stream.
			WriteByte(AMF3TypeCode.Null);
		}

		public void WriteAMF3Bool(bool value)
		{
			WriteByte( (byte)(value ? AMF3TypeCode.BooleanTrue : AMF3TypeCode.BooleanFalse));
		}

        public void WriteAMF3Array(Array value)
		{
            if (!_objectReferences.ContainsKey(value))
			{
                _objectReferences.Add(value, _objectReferences.Count);
                int handle = value.Length;
				handle = handle << 1;
				handle = handle | 1;
				WriteAMF3IntegerData(handle);
				WriteAMF3UTF(string.Empty);//hash name
                for (int i = 0; i < value.Length; i++)
				{
                    WriteAMF3Data(value.GetValue(i));
				}
			}
			else
			{
                int handle = (int)_objectReferences[value];
				handle = handle << 1;
				WriteAMF3IntegerData(handle);
			}
		}

		public void WriteAMF3Array(IList value)
		{
            if (!_objectReferences.ContainsKey(value))
			{
				_objectReferences.Add(value, _objectReferences.Count);
				int handle = value.Count;
				handle = handle << 1;
				handle = handle | 1;
				WriteAMF3IntegerData(handle);
				WriteAMF3UTF(string.Empty);//hash name
				for(int i = 0; i < value.Count; i++)
				{
					WriteAMF3Data(value[i]);
				}
			}
			else
			{
				int handle = (int)_objectReferences[value];
				handle = handle << 1;
				WriteAMF3IntegerData(handle);
			}
		}

		public void WriteAMF3Dictionary(IDictionary value)
		{
            if (!_objectReferences.ContainsKey(value))
			{
				_objectReferences.Add(value, _objectReferences.Count);
                int handle = value.Count;
                handle = handle << 1;
                handle = handle | 1;
                WriteAMF3IntegerData(handle);

                WriteAMF3Bool(false);
                foreach (DictionaryEntry entry in value)
				{
                    WriteAMF3Data(entry.Key);
					WriteAMF3Data(entry.Value);
				}
				
			}
			else
			{
				int handle = (int)_objectReferences[value];
				handle = handle << 1;
				WriteAMF3IntegerData(handle);
			}
		}

		internal void WriteByteArray(ByteArray byteArray)
		{
			_objectReferences.Add(byteArray, _objectReferences.Count);
			WriteByte(AMF3TypeCode.ByteArray);
			int handle = (int)byteArray.Length;
			handle = handle << 1;
			handle = handle | 1;
			WriteAMF3IntegerData(handle);
			WriteBytes( byteArray.MemoryStream.ToArray() );
		}


        public void WriteAMF3IntVector(IList<int> value, bool isFixed)
        {
            if (!_objectReferences.ContainsKey(value))
            {
                _objectReferences.Add(value, _objectReferences.Count);
                int handle = value.Count;
                handle = handle << 1;
                handle = handle | 1;
                WriteAMF3IntegerData(handle);
                WriteAMF3IntegerData(isFixed ? 1 : 0);
                for (int i = 0; i < value.Count; i++)
                {
                    WriteInt32(value[i]);
                }
            }
            else
            {
                int handle = (int)_objectReferences[value];
                handle = handle << 1;
                WriteAMF3IntegerData(handle);
            }
        }


        public void WriteAMF3UIntVector(IList<uint> value, bool isFixed)
        {
            if (!_objectReferences.ContainsKey(value))
            {
                _objectReferences.Add(value, _objectReferences.Count);
                int handle = value.Count;
                handle = handle << 1;
                handle = handle | 1;
                WriteAMF3IntegerData(handle);
                WriteAMF3IntegerData(isFixed ? 1 : 0);
                for (int i = 0; i < value.Count; i++)
                {
                    WriteInt32((int)value[i]);
                }
            }
            else
            {
                int handle = (int)_objectReferences[value];
                handle = handle << 1;
                WriteAMF3IntegerData(handle);
            }
        }


        public void WriteAMF3DoubleVector(IList<double> value,bool isFixed)
        {
            if (!_objectReferences.ContainsKey(value))
            {
                _objectReferences.Add(value, _objectReferences.Count);
                int handle = value.Count;
                handle = handle << 1;
                handle = handle | 1;
                WriteAMF3IntegerData(handle);
                WriteAMF3IntegerData(isFixed?1:0);
                for (int i = 0; i < value.Count; i++)
                {
                    WriteDouble(value[i]);
                }
            }
            else
            {
                int handle = (int)_objectReferences[value];
                handle = handle << 1;
                WriteAMF3IntegerData(handle);
            }
        }

        public void WriteAMF3ObjectVector(IList value)
        {
            Type listItemType = ReflectionUtils.GetListItemType(value.GetType());
            WriteAMF3ObjectVector(listItemType.FullName, value);
        }

        private void WriteAMF3ObjectVector(string typeIdentifier, IList value)
        {
            if (!_objectReferences.ContainsKey(value))
            {
                _objectReferences.Add(value, _objectReferences.Count);
                int handle = value.Count;
                handle = handle << 1;
                handle = handle | 1;
                WriteAMF3IntegerData(handle);
                WriteAMF3IntegerData(value is Array ? 1 : 0);
                
                WriteAMF3UTF(typeIdentifier);
                for (int i = 0; i < value.Count; i++)
                {
                    WriteAMF3Data(value[i]);
                }
            }
            else
            {
                int handle = (int)_objectReferences[value];
                handle = handle << 1;
                WriteAMF3IntegerData(handle);
            }
        }
        private static UTF8Encoding utf8Encoding = new UTF8Encoding();
        public void WriteAMF3UTF(string value)
		{
			if( value == string.Empty )
			{
				WriteAMF3IntegerData(1);
			}
			else
			{
                if (!_stringReferences.ContainsKey(value))
				{
					_stringReferences.Add(value, _stringReferences.Count);
				
					int byteCount = utf8Encoding.GetByteCount(value);
					int handle = byteCount;
					handle = handle << 1;
					handle = handle | 1;
					WriteAMF3IntegerData(handle);
					byte[] buffer = utf8Encoding.GetBytes(value);
					if (buffer.Length > 0)
						Write(buffer);
				}
				else
				{
					int handle = (int)_stringReferences[value];
					handle = handle << 1;
					WriteAMF3IntegerData(handle);
				}
			}
		}
        
        public void WriteAMF3String(string value)
		{
			WriteByte(AMF3TypeCode.String);
			WriteAMF3UTF(value);
		}

        public void WriteAMF3DateTime(DateTime value)
		{
            if (!_objectReferences.ContainsKey(value))
			{
				_objectReferences.Add(value, _objectReferences.Count);
				int handle = 1;
				WriteAMF3IntegerData(handle);

				// Write date (milliseconds from 1970).
				DateTime timeStart = new DateTime(1970, 1, 1, 0, 0, 0);

                value = value.ToUniversalTime();

                TimeSpan span = value.Subtract(timeStart);
				long milliSeconds = (long)span.TotalMilliseconds;
				//long date = BitConverter.DoubleToInt64Bits((double)milliSeconds);
				//this.WriteLong(date);
                WriteDouble((double)milliSeconds);
			}
			else
			{
				int handle = (int)_objectReferences[value];
				handle = handle << 1;
				WriteAMF3IntegerData(handle);
			}
		}

		private void WriteAMF3IntegerData(int value)
		{
			//Sign contraction - the high order bit of the resulting value must match every bit removed from the number
			//Clear 3 bits 
			value &= 0x1fffffff;
			if(value < 0x80)
				this.WriteByte((byte)value);
			else
				if(value < 0x4000)
			{
					this.WriteByte((byte)(value >> 7 & 0x7f | 0x80));
					this.WriteByte((byte)(value & 0x7f));
			}
			else
				if(value < 0x200000)
			{
				this.WriteByte((byte)(value >> 14 & 0x7f | 0x80));
				this.WriteByte((byte)(value >> 7 & 0x7f | 0x80));
				this.WriteByte((byte)(value & 0x7f));
			} 
			else
			{
				this.WriteByte((byte)(value >> 22 & 0x7f | 0x80));
				this.WriteByte((byte)(value >> 15 & 0x7f | 0x80));
				this.WriteByte((byte)(value >> 8 & 0x7f | 0x80));
				this.WriteByte((byte)(value & 0xff));
			}
		}

		public void WriteAMF3Int(int value)
		{
			if(value >= -268435456 && value <= 268435455)//check valid range for 29bits
			{
				WriteByte(AMF3TypeCode.Integer);
				WriteAMF3IntegerData(value);
			}
			else
			{
				//overflow condition would occur upon int conversion
				WriteAMF3Double((double)value);
			}
		}

		public void WriteAMF3Double(double value)
		{
			WriteByte(AMF3TypeCode.Number);
			//long tmp = BitConverter.DoubleToInt64Bits( double.Parse(value.ToString()) );
            WriteDouble(value);
		}


        public void WriteAMF3XmlDocument(XmlDocument value)
		{
			WriteByte(AMF3TypeCode.Xml);
            string xml = string.Empty;
            if (value.DocumentElement != null && value.DocumentElement.OuterXml != null)
                xml = value.DocumentElement.OuterXml;
            if (xml == string.Empty)
            {
                WriteAMF3IntegerData(1);
            }
            else
            {
                if (!_objectReferences.ContainsKey(value))
                {
                    _objectReferences.Add(value, _objectReferences.Count);
                    UTF8Encoding utf8Encoding = new UTF8Encoding();
                    int byteCount = utf8Encoding.GetByteCount(xml);
                    int handle = byteCount;
                    handle = handle << 1;
                    handle = handle | 1;
                    WriteAMF3IntegerData(handle);
                    byte[] buffer = utf8Encoding.GetBytes(xml);
                    if (buffer.Length > 0)
                        Write(buffer);
                }
                else
                {
                    int handle = (int)_objectReferences[value];
                    handle = handle << 1;
                    WriteAMF3IntegerData(handle);
                }
            }
		}

        public void WriteAMF3Object(object value)
		{
            if (!_objectReferences.ContainsKey(value))
			{
				_objectReferences.Add(value, _objectReferences.Count);

				ClassDefinition classDefinition = GetClassDefinition(value);
                if (classDefinition == null)
                {
                    DebugX.LogError("serializing:{0}", value.GetType().FullName);
                    return;
                }
                if (_classDefinitionReferences.ContainsKey(classDefinition))
                {
                    //Existing class-def
                    int handle = (int)_classDefinitionReferences[classDefinition];//handle = classRef 0 1
                    handle = handle << 2;
                    handle = handle | 1;
                    WriteAMF3IntegerData(handle);
                }
                else
				{//inline class-def
					
					//classDefinition = CreateClassDefinition(value);
                    _classDefinitionReferences.Add(classDefinition, _classDefinitionReferences.Count);
					//handle = memberCount dynamic externalizable 1 1
					int handle = classDefinition.MemberCount;
					handle = handle << 1;
					handle = handle | (classDefinition.IsDynamic ? 1 : 0);
					handle = handle << 1;
					handle = handle | (classDefinition.IsExternalizable ? 1 : 0);
					handle = handle << 2;
					handle = handle | 3;
					WriteAMF3IntegerData(handle);
					WriteAMF3UTF(classDefinition.ClassName);
					for(int i = 0; i < classDefinition.MemberCount; i++)
					{
						string key = classDefinition.Members[i].Name;
						WriteAMF3UTF(key);
					}
				}
				//write inline object
				if( classDefinition.IsExternalizable )
				{
					if( value is IExternalizable )
					{
						IExternalizable externalizable = value as IExternalizable;
						DataOutput dataOutput = new DataOutput(this);
						externalizable.WriteExternal(dataOutput);
					}
					else
					{
					    throw new NotImplementedException(classDefinition.ClassName + " must is IExternalizable");
					}
				}
				else
				{
                    Type type = value.GetType();
                    IObjectProxy proxy = ObjectProxyRegistry.GetObjectProxy(type);

					for(int i = 0; i < classDefinition.MemberCount; i++)
					{
                        object memberValue = proxy.GetValue(value, classDefinition.Members[i]);
                        WriteAMF3Data(memberValue);
					}

					if(classDefinition.IsDynamic)
					{
						IDictionary dictionary = value as IDictionary;
						foreach(DictionaryEntry entry in dictionary)
						{
							WriteAMF3UTF(entry.Key.ToString());
							WriteAMF3Data(entry.Value);
						}
						WriteAMF3UTF(string.Empty);
					}
				}
			}
			else
			{
				//handle = objectRef 0
				int handle = (int)_objectReferences[value];
				handle = handle << 1;
				WriteAMF3IntegerData(handle);
			}
		}

		private ClassDefinition GetClassDefinition(object obj)
		{
            ClassDefinition classDefinition = null;
            if (obj is ASObject)
            {
                ASObject asObject = obj as ASObject;
                if (asObject.IsTypedObject)
                    classDefinitions.TryGetValue(asObject.TypeName, out classDefinition);
                if (classDefinition != null)
                    return classDefinition;

                IObjectProxy proxy = ObjectProxyRegistry.GetObjectProxy(typeof(ASObject));
                classDefinition = proxy.GetClassDefinition(obj);
                if (asObject.IsTypedObject)
                {
                    //Only typed ASObject class definitions are cached.
                    classDefinitions[asObject.TypeName] = classDefinition;
                }
            }
            else
            {
                string typeName = obj.GetType().FullName;
                if( !classDefinitions.TryGetValue(typeName, out classDefinition))
                {
                    IObjectProxy proxy = ObjectProxyRegistry.GetObjectProxy(obj.GetType());
                    classDefinition = proxy.GetClassDefinition(obj);
                    classDefinitions[typeName] = classDefinition;
                }
            }

             return classDefinition;
		}
	}
}