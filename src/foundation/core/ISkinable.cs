using UnityEngine;

namespace foundation
{
    /// <summary>
    ///  可定义皮肤
    ///  一般用于代理类,如一个皮肤嵌套复制,由代理类来做简单封装
    /// </summary>
    public interface ISkinable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        GameObject skin
        {
            get;
            set;
        }
    }
}
