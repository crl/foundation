using System;
using System.Collections;
using System.ComponentModel;

namespace foundation
{

    class AMF3ObjectWriter : IAMFWriter
	{
		public AMF3ObjectWriter()
		{
		}
		#region IAMFWriter Members

		public bool IsPrimitive{ get{return false;} }

		public void WriteData(AMFWriter writer, object data)
		{
            IList list = data as IList;
            if (list != null )
			{			
				writer.WriteByte(AMF3TypeCode.Array);
                writer.WriteAMF3Array(list);
                return;
			}

            IListSource listSource = data as IListSource;
            if (listSource != null)
            {
                writer.WriteByte(AMF3TypeCode.Array);
                writer.WriteAMF3Array(listSource.GetList());
                return;
            }

            IDictionary dictionary = data as IDictionary;
            if (dictionary != null)
			{
                writer.WriteByte(AMF3TypeCode.Dictionary);
                writer.WriteAMF3Dictionary(dictionary);
				return;
			}
			if(data is Exception)
			{
				writer.WriteByte(AMF3TypeCode.Object);
                writer.WriteAMF3Object(data);
				return;
			}
			if( data is IExternalizable )
			{
				writer.WriteByte(AMF3TypeCode.Object);
				writer.WriteAMF3Object(data);
				return;
			}
            if (data is IEnumerable)
			{
                ArrayList tmp = new ArrayList();
                foreach (object element in (data as IEnumerable))
                {
                    tmp.Add(element);
                }
               
                writer.WriteByte(AMF3TypeCode.Array);
                writer.WriteAMF3Array(tmp);
            
                return;
			}
			writer.WriteByte(AMF3TypeCode.Object);
			writer.WriteAMF3Object(data);
		}

		#endregion
	}
}
