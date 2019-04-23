namespace foundation
{
    public class Parabola
    {
        private static float g = 9.8f;

        public static float easeIn(float t, float b,
            float c, float d)
        {
            float v0 = 0.5f*g*d + c/d;
            return b + v0*t - 0.5f*g*t*t;
        }

        public static float easeOut(float t, float b,
            float c, float d)
        {
            return easeIn(t, b, c, d);
        }


        public static float easeInOut(float t, float b,
            float c, float d)
        {
            return easeIn(t, b, c, d);
        }
    }
}
