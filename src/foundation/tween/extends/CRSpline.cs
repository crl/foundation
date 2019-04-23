using UnityEngine;

namespace foundation
{
    public class CRSpline
    {
        private Vector3[] pts;

        public CRSpline(Vector3[] pts)
        {
            this.pts = pts;
        }

        public Vector3 Interp(float t)
        {
            return GetPoint(pts, t);
        }


        public static Vector3 GetPoint(Vector3[] pts, float t)
        {
            int numSections = pts.Length - 3;
            int tSec = (int)Mathf.Floor(t * numSections);
            int currPt = numSections - 1;
            if (currPt > tSec)
            {
                currPt = tSec;
            }
            float u = t * numSections - currPt;

            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];

            return .5f * (
                           (-a + 3f * b - 3f * c + d) * (u * u * u)
                           + (2f * a - 5f * b + 4f * c - d) * (u * u)
                           + (-a + c) * u
                           + 2f * b
                       );
        }



        public static void DrawGizmosCurved(Vector3[] waypoints)
        {
            //helper array for curved paths, includes control points for waypoint array
            Vector3[] gizmoPoints = new Vector3[waypoints.Length + 2];
            waypoints.CopyTo(gizmoPoints, 1);
            gizmoPoints[0] = waypoints[1];
            gizmoPoints[gizmoPoints.Length - 1] = gizmoPoints[gizmoPoints.Length - 2];

            Vector3[] drawPs;
            Vector3 currPt;

            //store draw points
            int subdivisions = gizmoPoints.Length * 10;
            drawPs = new Vector3[subdivisions + 1];
            for (int i = 0; i <= subdivisions; ++i)
            {
                float pm = i / (float)subdivisions;
                currPt = GetPoint(gizmoPoints, pm);
                drawPs[i] = currPt;
            }

            //draw path
            Vector3 prevPt = drawPs[0];
            for (int i = 1; i < drawPs.Length; ++i)
            {
                currPt = drawPs[i];
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
        }
    }
}