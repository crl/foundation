using foundation;
using System;

namespace foundation
{
    class AMF3ExternalizableReader : IReflectionOptimizer
	{

        public AMF3ExternalizableReader()
		{
		}

        #region IReflectionOptimizer Members

        public object CreateInstance()
        {
            throw new NotImplementedException();
        }

        public object ReadData(AMFReader reader, ClassDefinition classDefinition)
        {
            object instance = ObjectFactory.CreateInstance(classDefinition.ClassName);
            if (instance == null)
            {
                
            }
            reader.AddAMF3ObjectReference(instance);
            if (instance is IExternalizable)
            {
                IExternalizable externalizable = instance as IExternalizable;
                DataInput dataInput = new DataInput(reader);
                externalizable.ReadExternal(dataInput);
                return instance;
            }
            else
            {
               
            }
            return instance;
        }

        #endregion
    }
}
