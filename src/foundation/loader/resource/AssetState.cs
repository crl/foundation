namespace foundation
{
    public enum AssetState
    {
        NONE,// 未加载
        PARSING,//解析状态
        PARSED,//解析状态

        LOADING,// 加载中
        READY,// 已加载
        FAILD,// 加载失败
        DISPOSE//不具体使用,用于调试
    }
}
