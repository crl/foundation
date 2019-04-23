using System;
namespace foundation
{
    /// <summary>
    /// 可热更新;
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HotFixAttribute : Attribute
    {
    }
    /// <summary>
    /// 强制不可热更新
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HotFixIgnoreAttribute : Attribute
    {
    }
}