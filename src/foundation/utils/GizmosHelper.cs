using UnityEngine;

namespace foundation
{
    public class GizmosHelper
    {
        public static void DrawCircle(Vector3 position, float radius, Color color)
        {
            Gizmos.color = color;
            int num = 60;

            for (int i = 0; i < num; i++)
            {
                float fromAngle = Mathf.Deg2Rad * i * (360 / num);
                float toAngle = Mathf.Deg2Rad * (i + 1) * (360 / num);

                Gizmos.DrawLine(
                    new Vector3(Mathf.Cos(fromAngle), Mathf.Sin(fromAngle), 0) * radius + position,
                    new Vector3(Mathf.Cos(toAngle), Mathf.Sin(toAngle), 0) * radius + position
                );
            }
        }

        public static void DrawArrow(Vector3 beginPosition, Vector3 direction, Color color)
        {
            if (direction.magnitude <= 0) return;

            float arrowHeadLength = 0.25f;
            float arrowHeadAngle = 20.0f;
            Vector3 endPosition = beginPosition + direction;

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
            Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;

            Gizmos.color = color;
            Gizmos.DrawLine(beginPosition, endPosition);
            Gizmos.DrawLine(endPosition, endPosition + right * arrowHeadLength * direction.magnitude);
            Gizmos.DrawLine(endPosition, endPosition + left * arrowHeadLength * direction.magnitude);
            Gizmos.DrawLine(endPosition, endPosition + up * arrowHeadLength * direction.magnitude);
            Gizmos.DrawLine(endPosition, endPosition + down * arrowHeadLength * direction.magnitude);
        }
        /// <summary>
        /// Draws curved gizmo lines between waypoints, taken and modified from HOTween.
        /// <summary>
        //http://code.google.com/p/hotween/source/browse/trunk/Holoville/HOTween/Core/Path.cs
        public static void DrawCurved(Vector3[] waypoints)
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


        /// <summary>
        /// Gets the point on the curve at a given percentage (0-1). Taken and modified from HOTween.
        /// <summary>
        //http://code.google.com/p/hotween/source/browse/trunk/Holoville/HOTween/Core/Path.cs
        public static Vector3 GetPoint(Vector3[] gizmoPoints, float t)
        {
            int numSections = gizmoPoints.Length - 3;
            int tSec = (int)Mathf.Floor(t * numSections);
            int currPt = numSections - 1;
            if (currPt > tSec)
            {
                currPt = tSec;
            }
            float u = t * numSections - currPt;

            Vector3 a = gizmoPoints[currPt];
            Vector3 b = gizmoPoints[currPt + 1];
            Vector3 c = gizmoPoints[currPt + 2];
            Vector3 d = gizmoPoints[currPt + 3];

            return .5f * (
                           (-a + 3f * b - 3f * c + d) * (u * u * u)
                           + (2f * a - 5f * b + 4f * c - d) * (u * u)
                           + (-a + c) * u
                           + 2f * b
                       );
        }
    }
}