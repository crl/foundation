namespace foundation
{
    public class Bounce
    {
        public static float easeIn(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            return end - easeOut(0, end, d - value) + start;
        }

        public static float easeOut(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < (1 / 2.75f))
            {
                return end * (7.5625f * value * value) + start;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return end * (7.5625f * (value) * value + .75f) + start;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return end * (7.5625f * (value) * value + .9375f) + start;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return end * (7.5625f * (value) * value + .984375f) + start;
            }
        }



        public static float easeInOut(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            if (value<d*0.5f) return easeIn(0, end, value*2)*0.5f + start;
            else return easeOut(0, end, value*2 - d)*0.5f + end*0.5f + start;
        }
}
}