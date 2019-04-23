namespace foundation
{
    /// <summary>
    /// 可发布事件者
    /// </summary>
    public interface INotifier
    {
        bool simpleDispatch(string type, object data = null);
    }
}
