using System.Collections;
using UnityEngine;

namespace foundation
{
    public static class TrailRendererExtension
    {
        public static void Reset(this TrailRenderer trail, MonoBehaviour instance)
        {
            trail.Clear();
            instance.StartCoroutine(ResetTrail(trail));
        }

        private static IEnumerator ResetTrail(TrailRenderer trail)
        {
            var trailTime=1.0f;
            if (trail)
            {
                trailTime = trail.time;
                trail.time = -1;
            }

            yield return new WaitForEndOfFrame();

            if (trail)
            {
                trail.time = trailTime;
            }
        }
    }
}