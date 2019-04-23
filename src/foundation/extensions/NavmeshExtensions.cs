using UnityEngine;
using UnityEngine.AI;

namespace foundation
{
    public static class NavmeshUtils
    {
        private static NavMeshHit hit;

        public static Vector3 GetNear(Vector3 position, int minDistance = 1, int step = 1)
        {
            if (step < 1)
            {
                step = 1;
            }
            Vector3 result = Vector3.zero;
            for (int i = 0; i < step; i++)
            {
                int v = minDistance + i;
                if (NavMesh.SamplePosition(position, out hit, v, NavMesh.AllAreas))
                {
                    result = hit.position;
                    break;
                }
            }
            return result;
        }
    }
}