namespace foundation
{
    /// <summary>
    /// 希望加载后返回什么格式的数据.
    /// </summary>
    public enum LoaderXDataType
    {
        BYTES,
        EDITORRESOURCE,
        RESOURCE,
        TEXTURE,
        MANIFEST,
        ASSETBUNDLE,
        PREFAB = ASSETBUNDLE,
        AMF,
        POST,
        GET,

        //AMFPREFAB
        NONE
    }
}