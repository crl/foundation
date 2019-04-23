using UnityEngine;

namespace foundation
{
    public class AnimationUtils
    {
        public static void CrossFade(Animation ani, string aniName)
        {
            if (ani == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(aniName))
            {
                return;
            }

            if (ani[aniName] != null)
            {
                ani.CrossFade(aniName);
            }
        }
    }
}