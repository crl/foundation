namespace foundation
{
    /// <summary>
    /// 可被调整大小
    /// 如场景的各个部件的变化,会调用统一的resize;
    /// </summary>
    public interface IResizeable
    {
        /// <summary>
        ///  可被resize的对像
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void onResize(float width, float height);
    }
}
