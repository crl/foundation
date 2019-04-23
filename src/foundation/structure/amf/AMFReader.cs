using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace foundation
{
    public class AMFReader : BinaryReader
    {
        protected List<object> _objectReferences;
        protected List<object> _stringReferences;
        protected List<object> _classDefinitions;

        private static IAMFReader[] AmfTypeTable = new IAMFReader[]
		{
				new AMF3NullReader(), /*0*/
				new AMF3NullReader(), /*1*/
				new AMF3BooleanFalseReader(), /*2*/
				new AMF3BooleanTrueReader(), /*3*/
				new AMF3IntegerReader(), /*4*/
				new AMF3NumberReader(), /*5*/
				new AMF3StringReader(), /*6*/
				new AMF3XmlReader(), /*7*/
				new AMF3DateTimeReader(), /*8*/
				new AMF3ArrayReader(),  /*9*/
                new AMF3ObjectReader(),/*10*/
				new AMF3XmlReader(), /*11*/
				new AMF3ByteArrayReader(), /*12*/
				new AMF3IntVectorReader(), /*13*/
				new AMF3UIntVectorReader(), /*14*/
				new AMF3DoubleVectorReader(), /*15*/
				new AMF3ObjectVectorReader(), /*16*/
				new AMF3DictionaryReader(),/*17*/
		};

        public AMFReader(Stream stream)
            : base(stream)
        {

            _objectReferences = new List<object>(15);
            _stringReferences = new List<object>(15);
            _classDefinitions = new List<object>(2);
        }

        public override ushort ReadUInt16()
        {
            //Read the next 2 bytes, shift and add.
            byte[] bytes = ReadBytes(2);
            return (ushort)(((bytes[0] & 0xff) << 8) | (bytes[1] & 0xff));
        }
        /// <summary>
        /// Reads a 2-byte signed integer from the current AMF stream using network byte order encoding and advances the position of the stream by two bytes.
        /// </summary>
        /// <returns>The 2-byte signed integer.</returns>
        public override short ReadInt16()
        {
            //Read the next 2 bytes, shift and add.
            byte[] bytes = ReadBytes(2);
            return (short)((bytes[0] << 8) | bytes[1]);
        }
        /// <summary>
        /// Reads an UTF-8 encoded String from the current AMF stream.
        /// </summary>
        /// <returns>The String value.</returns>
        public override string ReadString()
        {
            //Get the length of the string (first 2 bytes).
            int length = ReadUInt16();
            return ReadUTF(length);
        }
        /// <summary>
        /// Reads a Boolean value from the current AMF stream using network byte order encoding and advances the position of the stream by one byte.
        /// </summary>
        /// <returns>The Boolean value.</returns>
        public override bool ReadBoolean()
        {
            return base.ReadBoolean();
        }
        /// <summary>
        /// Reads a 4-byte signed integer from the current AMF stream using network byte order encoding and advances the position of the stream by four bytes.
        /// </summary>
        /// <returns>The 4-byte signed integer.</returns>
        public override int ReadInt32()
        {
            // Read the next 4 bytes, shift and add
            byte[] bytes = ReadBytes(4);
            int value = (int)((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
            return value;
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the current AMF stream.
        /// </summary>
        /// <returns>The 4-byte signed integer.</returns>
        public int ReadReverseInt()
        {
            byte[] bytes = this.ReadBytes(4);
            int val = 0;
            val += bytes[3] << 24;
            val += bytes[2] << 16;
            val += bytes[1] << 8;
            val += bytes[0];
            return val;
        }
        /// <summary>
        /// Reads a 3-byte signed integer from the current AMF stream using network byte order encoding and advances the position of the stream by three bytes.
        /// </summary>
        /// <returns>The 3-byte signed integer.</returns>
        public int ReadUInt24()
        {
            byte[] bytes = this.ReadBytes(3);
            int value = bytes[0] << 16 | bytes[1] << 8 | bytes[2];
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ReadUInt29()
        {
            int value;

            // Each byte must be treated as unsigned
            int b = ReadByte() & 0xFF;

            if (b < 128)
                return b;

            value = (b & 0x7F) << 7;
            b = ReadByte() & 0xFF;

            if (b < 128)
                return (value | b);

            value = (value | (b & 0x7F)) << 7;
            b = ReadByte() & 0xFF;

            if (b < 128)
                return (value | b);

            value = (value | (b & 0x7F)) << 8;
            b = ReadByte() & 0xFF;

            return (value | b);
        }

        /// <summary>
        /// Reads an 8-byte IEEE-754 double precision floating point number from the current AMF stream using network byte order encoding and advances the position of the stream by eight bytes.
        /// </summary>
        /// <returns>The 8-byte double precision floating point number.</returns>
        public override double ReadDouble()
        {
            byte[] bytes = ReadBytes(8);
            byte[] reverse = new byte[8];
            //Grab the bytes in reverse order 
            for (int i = 7, j = 0; i >= 0; i--, j++)
            {
                reverse[j] = bytes[i];
            }
            double value = BitConverter.ToDouble(reverse, 0);
            return value;
        }
        /// <summary>
        /// Reads a single-precision floating point number from the current AMF stream using network byte order encoding and advances the position of the stream by eight bytes.
        /// </summary>
        /// <returns>The single-precision floating point number.</returns>
        public float ReadFloat()
        {
            byte[] bytes = this.ReadBytes(4);
            byte[] invertedBytes = new byte[4];
            //Grab the bytes in reverse order from the backwards index
            for (int i = 3, j = 0; i >= 0; i--, j++)
            {
                invertedBytes[j] = bytes[i];
            }
            float value = BitConverter.ToSingle(invertedBytes, 0);
            return value;
        }


        public object ReadAMF3Data()
        {
            byte typeCode = this.ReadByte();
            return this.ReadAMF3Data(typeCode);
        }

        public object ReadAMF3Data(byte typeMarker)
        {
            //DebugX.Log(typeMarker+"="+this.BaseStream.Position);
            return AmfTypeTable[typeMarker].ReadData(this);
        }

        public void AddAMF3ObjectReference(object instance)
        {
            _objectReferences.Add(instance);
        }

        public object ReadAMF3ObjectReference(int index)
        {
            return _objectReferences[index];
        }

        public int ReadAMF3IntegerData()
        {
            int acc = ReadByte();
            int tmp;
            if (acc < 128)
                return acc;
            else
            {
                acc = (acc & 0x7f) << 7;
                tmp = this.ReadByte();
                if (tmp < 128)
                    acc = acc | tmp;
                else
                {
                    acc = (acc | tmp & 0x7f) << 7;
                    tmp = this.ReadByte();
                    if (tmp < 128)
                        acc = acc | tmp;
                    else
                    {
                        acc = (acc | tmp & 0x7f) << 8;
                        tmp = this.ReadByte();
                        acc = acc | tmp;
                    }
                }
            }

            int mask = 1 << 28; // mask
            int r = -(acc & mask) | acc;
            return r;
        }

        public int ReadAMF3Int()
        {
            int intData = ReadAMF3IntegerData();
            return intData;
        }

        public DateTime ReadAMF3Date()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0);
            handle = handle >> 1;
            if (inline)
            {
                double milliseconds = this.ReadDouble();
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0);

                DateTime date = start.AddMilliseconds(milliseconds);

                AddAMF3ObjectReference(date);
                return date;
            }
            else
            {
                return (DateTime)ReadAMF3ObjectReference(handle);
            }
        }

        internal void AddStringReference(string str)
        {
            _stringReferences.Add(str);
        }

        internal string ReadStringReference(int index)
        {
            return _stringReferences[index] as string;
        }

        public string ReadAMF3String()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0);
            handle = handle >> 1;
            if (inline)
            {
                int length = handle;
                if (length == 0)
                    return string.Empty;
                string str = ReadUTF(length);
                AddStringReference(str);
                return str;
            }
            else
            {
                return ReadStringReference(handle);
            }
        }

        public XmlDocument ReadAMF3XmlDocument()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0);
            handle = handle >> 1;
            string xml = string.Empty;
            if (inline)
            {
                if (handle > 0)//length
                    xml = this.ReadUTF(handle);
                AddAMF3ObjectReference(xml);
            }
            else
            {
                xml = ReadAMF3ObjectReference(handle) as string;
            }
            XmlDocument xmlDocument = new XmlDocument();
            if (xml != null && xml != string.Empty)
                xmlDocument.LoadXml(xml);
            return xmlDocument;
        }

        private static UTF8Encoding utf8 = new UTF8Encoding(false, false);
        public string ReadUTF(int length)
        {
            if (length == 0)
                return string.Empty;
            return utf8.GetString(this.ReadBytes(length));
        }


        public ByteArray ReadAMF3ByteArray()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0);
            handle = handle >> 1;
            if (inline)
            {
                int length = handle;
                byte[] buffer = ReadBytes(length);
                ByteArray ba = new ByteArray(buffer);
                AddAMF3ObjectReference(ba);
                return ba;
            }
            else
                return ReadAMF3ObjectReference(handle) as ByteArray;
        }

        public object ReadAMF3Array()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0); handle = handle >> 1;
            if (inline)
            {

                Dictionary<string, object> hashtable = null;

                string key = ReadAMF3String();
                while (key != null && key != string.Empty)
                {
                    if (hashtable == null)
                    {
                        hashtable = new Dictionary<string, object>();
                        AddAMF3ObjectReference(hashtable);
                    }
                    object value = ReadAMF3Data();
                    hashtable.Add(key, value);
                    key = ReadAMF3String();
                }
                //Not an associative array
                if (hashtable == null)
                {
                    object[] array = new object[handle];
                    AddAMF3ObjectReference(array);
                    for (int i = 0; i < handle; i++)
                    {
                        //Grab the type for each element.
                        byte typeCode = this.ReadByte();
                        object value = ReadAMF3Data(typeCode);
                        array[i] = value;
                    }
                    return array;
                }
                else
                {
                    for (int i = 0; i < handle; i++)
                    {
                        object value = ReadAMF3Data();
                        hashtable.Add(i.ToString(), value);
                    }
                    return hashtable;
                }
            }
            else
            {
                return ReadAMF3ObjectReference(handle);
            }
        }

        public IList ReadAMF3IntVector()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0); handle = handle >> 1;
            if (inline)
            {
                int @fixed = ReadAMF3IntegerData();
                IList list;
                if (@fixed==1)
                {
                    list = new int[handle];
                    AddAMF3ObjectReference(list);

                    for (int i = 0; i < handle; i++)
                    {
                        list[i] = ReadInt32();
                    }
                }
                else
                {
                    list=new List<int>(handle);
                    AddAMF3ObjectReference(list);

                    for (int i = 0; i < handle; i++)
                    {
                        list.Add(ReadInt32());
                    }
                }
             
                return list;
            }
            else
            {
                return ReadAMF3ObjectReference(handle) as IList;
            }
        }

        public IList ReadAMF3UIntVector()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0); handle = handle >> 1;
            if (inline)
            {
                int @fixed = ReadAMF3IntegerData();
                IList list;
                if (@fixed == 1)
                {
                    list = new uint[handle];
                    AddAMF3ObjectReference(list);
                    for (int i = 0; i < handle; i++)
                    {
                        list[i] = (uint)ReadInt32();
                    }
                }
                else
                {
                    list= new List<uint>(handle);
                    AddAMF3ObjectReference(list);
                    for (int i = 0; i < handle; i++)
                    {
                        list.Add((uint)ReadInt32());
                    }
                }
                return list;
            }
            else
            {
                return ReadAMF3ObjectReference(handle) as IList;
            }
        }

        public IList ReadAMF3DoubleVector()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0); handle = handle >> 1;
            if (inline)
            {
                int @fixed = ReadAMF3IntegerData();
                IList list;
                if (@fixed == 1)
                {
                    list = new double[handle];
                    AddAMF3ObjectReference(list);
                    for (int i = 0; i < handle; i++)
                    {
                        list[i] = ReadDouble();
                    }
                }
                else
                {
                    list = new List<double>(handle);
                    AddAMF3ObjectReference(list);
                    for (int i = 0; i < handle; i++)
                    {
                        list.Add(ReadDouble());
                    }
                }
                return list;
            }
            else
            {
                return ReadAMF3ObjectReference(handle) as IList;
            }
        }


        public object ReadAMF3ObjectVector()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0); handle = handle >> 1;
            if (inline)
            {
                //List<object> list = new List<object>(handle);
                int @fixed = ReadAMF3IntegerData();
                string typeIdentifier = ReadAMF3String();

                Type type=null;
                if (!string.Empty.Equals(typeIdentifier))
                {
                    type = ObjectFactory.Locate(typeIdentifier);
                }
                IList list;
                if (@fixed == 1)
                {
                    if (type!=null)
                    {
                        list = Array.CreateInstance(ObjectFactory.Locate(typeIdentifier), handle);
                    }
                    else
                    {
                        type = typeof (object);
                        list = new object[handle];
                    }
                    AddAMF3ObjectReference(list);
                    for (int i = 0; i < handle; i++)
                    {
                        byte typeCode = this.ReadByte();
                        object obj = ReadAMF3Data(typeCode);
                        list[i]= Convert.ChangeType(obj, type); ;
                    }
                    return list;
                }
                else
                {
                    if (type!=null)
                    {
                        list =ReflectionUtils.CreateGeneric(typeof (List<>), ObjectFactory.Locate(typeIdentifier)) as IList;
                    }
                    else
                    {
                        type = typeof (object);
                        list = new List<object>();
                    }
                    AddAMF3ObjectReference(list);

                    for (int i = 0; i < handle; i++)
                    {
                        byte typeCode = this.ReadByte();
                        object obj = ReadAMF3Data(typeCode);
                        obj= Convert.ChangeType(obj, type);
                        list.Add(obj);
                    }
                    return list;
                }
            }
            else
            {
                return ReadAMF3ObjectReference(handle);
            }
        }


        public Array ReadAMF3ObjectArray()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0); handle = handle >> 1;
            if (inline)
            {
                //List<object> list = new List<object>(handle);
                //int @fixed = ReadAMF3IntegerData();
                string typeIdentifier = ReadAMF3String();
                Array list;
                if (!string.Empty.Equals(typeIdentifier))
                {
                    Type dstElementType = ObjectFactory.Locate(typeIdentifier);
                    list = Array.CreateInstance(dstElementType, handle);
                }
                else
                {
                    list = new object[handle];
                }
                AddAMF3ObjectReference(list);
                for (int i = 0; i < handle; i++)
                {
                    byte typeCode = this.ReadByte();
                    object obj = ReadAMF3Data(typeCode);
                    list.SetValue(obj,i);
                }
                //if (@fixed == 1)
                    //return list.GetType().GetMethod("AsReadOnly").Invoke(list, null) as IList;

                return list;
            }
            else
            {
                return ReadAMF3ObjectReference(handle) as Array;
            }
        }

        internal void AddClassReference(ClassDefinition classDefinition)
        {
            _classDefinitions.Add(classDefinition);
        }

        internal ClassDefinition ReadClassReference(int index)
        {
            return _classDefinitions[index] as ClassDefinition;
        }

        internal ClassDefinition ReadClassDefinition(int handle)
        {
            ClassDefinition classDefinition = null;
            //an inline object
            bool inlineClassDef = ((handle & 1) != 0); handle = handle >> 1;
            if (inlineClassDef)
            {
                //inline class-def
                string typeIdentifier = ReadAMF3String();
                //flags that identify the way the object is serialized/deserialized
                bool externalizable = ((handle & 1) != 0); handle = handle >> 1;
                bool dynamic = ((handle & 1) != 0); handle = handle >> 1;

                ClassMember[] members = null;
                if (handle > 0)
                {
                   members = new ClassMember[handle];
                    for (int i = 0; i < handle; i++)
                    {
                        string name = ReadAMF3String();
                        ClassMember classMember = new ClassMember(name, BindingFlags.Default, MemberTypes.Custom, null);
                        members[i] = classMember;
                    }
                }
                classDefinition = new ClassDefinition(typeIdentifier, members, externalizable, dynamic);
                AddClassReference(classDefinition);
            }
            else
            {
                //A reference to a previously passed class-def
                classDefinition = ReadClassReference(handle);
            }

            return classDefinition;
        }

        internal object ReadAMF3Object(ClassDefinition classDefinition)
        {
            //Console.WriteLine(classDefinition.ClassName + "=" + BaseStream.Position);

            object instance = null;
            if (!string.IsNullOrEmpty(classDefinition.ClassName))
            {
                instance = ObjectFactory.CreateInstance(classDefinition.ClassName);
            }
            else
            {
                instance = new ASObject();
            }
            if (instance == null)
            {
                instance = new ASObject(classDefinition.ClassName);
            }
            AddAMF3ObjectReference(instance);
            if (classDefinition.IsExternalizable)
            {
                if (instance is IExternalizable)
                {
                    IExternalizable externalizable = instance as IExternalizable;
                    DataInput dataInput = new DataInput(this);
                    externalizable.ReadExternal(dataInput);
                }
                else
                {
                    throw new Exception(classDefinition.ClassName + "是个IExternalizable类型");
                }
            }
            else
            {
                for (int i = 0; i < classDefinition.MemberCount; i++)
                {
                    string key = classDefinition.Members[i].Name;
                 
                    object value = ReadAMF3Data();
                    SetMember(instance, key, value);
                }
                if (classDefinition.IsDynamic)
                {
                    string key = ReadAMF3String();
                    while (key != null && key != string.Empty)
                    {
                        object value = ReadAMF3Data();
                        SetMember(instance, key, value);
                        key = ReadAMF3String();
                       
                    }
                }
            }
            return instance;
        }

        private int varI=0;
        public object ReadAMF3Object()
        {
            int handle = ReadAMF3IntegerData();
            bool inline = ((handle & 1) != 0); handle = handle >> 1;
            if (!inline)
            {
                //An object reference
                return ReadAMF3ObjectReference(handle);
            }
            else
            {
                varI++;
                ClassDefinition classDefinition = ReadClassDefinition(handle);
                object obj = ReadAMF3Object(classDefinition);
                varI--;

                if (notFoundMessage.Length > 0 && varI == 0)
                {
                    if (Application.isEditor)
                    {
                        Debug.Log(notFoundMessage.ToString());
                    }
                    //notFoundMessage.Length = 0;
                    notFoundMessage.Remove(0, notFoundMessage.Length);
                }

                return obj;
            }
        }

        internal void SetMember(object instance, string memberName, object value)
        {
            if (instance is ASObject)
            {
                ((ASObject) instance)[memberName] = value;
                return;
            }
            Type type = instance.GetType();

            FieldInfo fi = type.GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
            try
            {
                bool has = false;
                if (fi != null)
                {
                    if (value != null && ReflectionUtils.IsSubClass(value.GetType(), fi.FieldType))
                    {
                        fi.SetValue(instance, value);
                    }
                    else
                    {
                        value = Convert.ChangeType(value, fi.FieldType);
                        fi.SetValue(instance, value);
                    }
                    has = true;
                }
                else
                {
                    PropertyInfo mi = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance|BindingFlags.SetProperty);
                    if (mi != null)
                    {
                        //value = Convert.ChangeType(value, mi.PropertyType);del by wangershi 此行导致自定义enum转换失败
                        mi.SetValue(instance, value, null);
                        has = true;
                    }
                }
                if (has == false && Application.isEditor)
                {
                    notFoundMessage.AppendLine("amf class:" + instance.GetType().Name + " field:" + memberName +
                                               " not found!");
                }
            }
            catch (Exception ex)
            {
                IAmfSetMember setMember= instance as IAmfSetMember;
                if (setMember != null)
                {
                    setMember.__AmfSetMember(memberName, value);
                }
                else
                {
                    string valueType = "null";
                    if (value != null)
                    {
                        valueType = value.GetType().ToString();
                    }
                    throw new Exception("amf class:" + instance.GetType().Name + " field:" + memberName +
                                        " newValue typeof：" +
                                      
                                        valueType + " msg:" + ex.Message);
                }
            }
        }

        private StringBuilder notFoundMessage=new StringBuilder();

        public Object ReadDictionary()
        {
            int refV = ReadUInt29();
            if ((refV & 1) == 0)
            { // This is a reference.
                return ReadAMF3ObjectReference(refV >> 1);
            }
            ReadBoolean(); // usingWeakTypes - irrelevant in Java.
            int len = (refV >> 1);

            Dictionary<Object, Object> dictionary = new Dictionary<object,object>();
            AddAMF3ObjectReference(dictionary); // Remember the object.

            for (int i = 0; i < len; i++)
            {
                Object key = ReadAMF3Data();
                Object value = ReadAMF3Data();
                dictionary.Add(key, value);
            }
            return dictionary;
        }


    }
}
