namespace foundation
{
    /// <summary>
    ///  工厂接口
    ///  用于创建实例时，进行的流程式的初始化过程
    /// </summary>
    public interface IFactory
    {
        object newInstance();
    }
}
