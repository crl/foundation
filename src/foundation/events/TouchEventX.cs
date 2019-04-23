namespace foundation
{
    public class TouchEventX : EventX
    {
        public float x;
        public float y;
        public float dx;
        public float dy;
        public static readonly string TOUCH_START="touchStart";
        new public static readonly string TOUCH_END="touchEnd";

        public TouchEventX(string type, float x, float y, float dx, float dy) : base(type, null)
        {
            this.x = x;
            this.y = y;
            this.dx = dx;
            this.dy = dy;
        }
    }
}
