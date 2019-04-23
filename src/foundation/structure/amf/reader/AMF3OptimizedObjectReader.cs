using System;
using System.Collections;
using foundation;

namespace foundation
{

	class AMF3OptimizedObjectReader : IAMFReader
	{
        Hashtable _optimizedReaders;

		public AMF3OptimizedObjectReader()
		{
            _optimizedReaders = new Hashtable();
		}

		#region IAMFReader Members

		public object ReadData(AMFReader reader)
		{
			int handle = reader.ReadAMF3IntegerData();
			bool inline = ((handle & 1) != 0 ); handle = handle >> 1;
			if (!inline)
			{
				//An object reference
				return reader.ReadAMF3ObjectReference(handle);
			}
			else
			{
				ClassDefinition classDefinition = reader.ReadClassDefinition(handle);
				object instance = null;
                IReflectionOptimizer reflectionOptimizer = _optimizedReaders[classDefinition.ClassName] as IReflectionOptimizer;
				if (reflectionOptimizer == null)
				{
					lock (_optimizedReaders)
					{
						if (classDefinition.IsTypedObject)
						{
							if (!_optimizedReaders.Contains(classDefinition.ClassName))
							{
								//Temporary reader
                                _optimizedReaders[classDefinition.ClassName] = new AMF3TempObjectReader();
								Type type = ObjectFactory.Locate(classDefinition.ClassName);
								if (type != null)
								{
									instance = ObjectFactory.CreateInstance(type);
                                    if (classDefinition.IsExternalizable)
                                    {
                                        reflectionOptimizer = new AMF3ExternalizableReader();
                                        _optimizedReaders[classDefinition.ClassName] = reflectionOptimizer;
                                        instance = reflectionOptimizer.ReadData(reader, classDefinition);
                                    }
                                    else
                                    {
                                        reader.AddAMF3ObjectReference(instance);
                                    }
								}
								else
								{
                                    reflectionOptimizer = new AMF3TypedASObjectReader(classDefinition.ClassName);
                                    _optimizedReaders[classDefinition.ClassName] = reflectionOptimizer;
                                    instance = reflectionOptimizer.ReadData(reader, classDefinition);
								}
							}
							else
							{
                                reflectionOptimizer = _optimizedReaders[classDefinition.ClassName] as IReflectionOptimizer;
								instance = reflectionOptimizer.ReadData(reader, classDefinition);
							}
						}
						else
						{
                            reflectionOptimizer = new AMF3TypedASObjectReader(classDefinition.ClassName);
                            _optimizedReaders[classDefinition.ClassName] = reflectionOptimizer;
                            instance = reflectionOptimizer.ReadData(reader, classDefinition);
						}
					}
				}
				else
				{
					instance = reflectionOptimizer.ReadData(reader, classDefinition);
				}
				return instance;
			}
		}

		#endregion
	}

    class AMF3TempObjectReader : IReflectionOptimizer
    {
        #region IReflectionOptimizer Members

        public object CreateInstance()
        {
            throw new NotImplementedException();
        }

        public object ReadData(AMFReader reader, ClassDefinition classDefinition)
        {
            object obj = reader.ReadAMF3Object(classDefinition);
            return obj;
        }

        #endregion
    }
}
