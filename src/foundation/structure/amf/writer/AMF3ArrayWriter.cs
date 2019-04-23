using System;
using System.Collections;
using System.Collections.Generic;

namespace foundation
{
    class AMF3ArrayWriter : IAMFWriter
	{
		public AMF3ArrayWriter()
		{
		}
		#region IAMFWriter Members

		public bool IsPrimitive{ get{ return false; } }

		public void WriteData(AMFWriter writer, object data)
		{

		    Type type=data.GetType().GetElementType();

		    if (type == typeof (object))
		    {
		        writer.WriteByte(AMF3TypeCode.Array);
		        writer.WriteAMF3Array(data as Array);
                return;
		    }
		    
            if(type==typeof(int))
		    {
		        writer.WriteByte(AMF3TypeCode.IntVector);

                IList rawData = (IList)data;
                List<int> list = new List<int>(rawData.Count);
                foreach (int item in rawData)
                {
                    list.Add(item);
                }

                writer.WriteAMF3IntVector(list,data is Array);
                return;
            }

            if (type == typeof(uint))
            {
                writer.WriteByte(AMF3TypeCode.UIntVector);

                IList rawData = (IList)data;
                List<uint> list = new List<uint>(rawData.Count);
                foreach (uint item in rawData)
                {
                    list.Add(item);
                }
                writer.WriteAMF3UIntVector(list,data is Array);
                return;
            }

            if (type == typeof(float))
            {
                writer.WriteByte(AMF3TypeCode.NumberVector);
                IList rawData = (IList) data;
                List<double> list=new List<double>(rawData.Count);
                foreach (float item in rawData)
                {
                    list.Add(Convert.ToDouble(item));
                }
                writer.WriteAMF3DoubleVector(list,data is Array);
                return;
            }

            if (type == typeof(double))
            {
                writer.WriteByte(AMF3TypeCode.NumberVector);
                IList rawData = (IList)data;
                List<double> list = new List<double>(rawData.Count);
                foreach (double item in rawData)
                {
                    list.Add(item);
                }
                writer.WriteAMF3DoubleVector(list,data is Array);
                return;
            }


            writer.WriteByte(AMF3TypeCode.ObjectVector);
            writer.WriteAMF3ObjectVector(data as IList);
        }

		#endregion
	}
}
