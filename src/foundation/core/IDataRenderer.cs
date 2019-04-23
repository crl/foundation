namespace foundation
{
    /// <summary>
    ///  数据接口,
    ///  可代理数据的显示及操作的类可实现此接口
    /// </summary>
    public interface IDataRenderer:IDataRenderer<object>
    {
    }

    public interface IDataRenderer<T>
    {
        /// <summary>
        /// 取得数据
        /// </summary>
        T data
        {
            get;
            set;
        }
    }
}
