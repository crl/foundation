namespace foundation
{
    public interface IReflectionOptimizer
	{

		object CreateInstance();
   
        object ReadData(AMFReader reader, ClassDefinition classDefinition);
	}
}
