using System;

namespace foundation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CMDAttribute : Attribute
    {
        public int code;

        public CMDAttribute(int code = 0)
        {
            this.code = code;
        }

    }
}
