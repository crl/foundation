using System;

namespace foundation
{
    [Serializable]
    public class KeyVector2
    {
        public string x, y;
        internal bool isEqual(KeyVector2 v)
        {
            return this.x == v.x && this.y == v.y;
        }
    }
}