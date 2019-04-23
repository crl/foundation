namespace foundation
{
    public class Quadratic
    {
        public static float easeIn(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        public static float easeOut(float start, float end, float value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        public static float easeInOut(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end*0.5f*value*value + start;
            value--;
            return -end*0.5f*(value*(value - 2) - 1) + start;

        }
    }
}



