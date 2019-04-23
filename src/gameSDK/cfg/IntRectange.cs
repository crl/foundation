using System;
using UnityEngine;

namespace gameSDK
{
    [Serializable]
    public class IntRectange
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public void CopyFrom(Rect rect)
        {
            this.x = (int) rect.x;
            this.y = (int) rect.y;
            this.width = (int) rect.width;
            this.height = (int) rect.height;
        }
    }


    [Serializable]
    public class IntPoint
    {
        public int x;
        public int y;

        public void CopyFrom(Vector2 v)
        {
            this.x = (int) v.x;
            this.y = (int) v.y;
        }
    }
}