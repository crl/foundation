using foundation;

namespace gameSDK
{
    public class AnimatorEventX : EventX
    {
        public static string ANIMATOR_MOVE = "ANIMATOR_MOVE";
        public static string ANIMATOR_IK = "ANIMATOR_IK";
        public AnimatorEventX(string type, object data) : base(type, data)
        {

        }
    }
}