using UnityEngine;

namespace foundation
{
    public interface IRayHitReceiver
    {
        bool OnRayHit(RaycastHit hit);

        bool TryRayHitMore(RaycastHit hit);

        void OnRayHitSelf();
    }
}