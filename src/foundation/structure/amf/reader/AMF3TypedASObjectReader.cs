using System;
using foundation;

namespace foundation
{
	
    class AMF3TypedASObjectReader : IReflectionOptimizer
	{
		string _typeIdentifier;

		public AMF3TypedASObjectReader(string typeIdentifier)
		{
			_typeIdentifier = typeIdentifier;
		}

        #region IReflectionOptimizer Members

        public object CreateInstance()
        {
            throw new NotImplementedException();
        }

        public object ReadData(AMFReader reader, ClassDefinition classDefinition)
        {
            ASObject aso = new ASObject(_typeIdentifier);
            reader.AddAMF3ObjectReference(aso);
            string key = reader.ReadAMF3String();
            aso.TypeName = _typeIdentifier;
            while (key != string.Empty)
            {
                object value = reader.ReadAMF3Data();
                aso.Add(key, value);
                key = reader.ReadAMF3String();
            }
            return aso;
        }

        #endregion
    }
}
