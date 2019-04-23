using UnityEngine;

namespace foundation
{
    enum PlaneSide
    {
        Left,
        Right,
        Bottom,
        Top,
        Near,
        Far
    }
    public static class Matrix4x4Extensions
    {
        // <summary>
        /// Return the position of the matrix
        /// </summary>
        public static Vector3 Position(this Matrix4x4 rMatrix)
        {
            /*
            Vector3 position;
            position.x = rMatrix.m03;
            position.y = rMatrix.m13;
            position.z = rMatrix.m23;
            */
            return rMatrix.GetColumn(3);
        }

        /// <summary>
        /// Return the rotation of the matrix
        /// </summary>
        public static Quaternion Rotation(this Matrix4x4 rMatrix)
        {
            /*
            Vector3 forward;
            forward.x = rMatrix.m02;
            forward.y = rMatrix.m12;
            forward.z = rMatrix.m22;

            Vector3 upwards;
            upwards.x = rMatrix.m01;
            upwards.y = rMatrix.m11;
            upwards.z = rMatrix.m21;

            return Quaternion.LookRotation(forward, upwards);
            */
            return Quaternion.LookRotation(rMatrix.GetColumn(2), rMatrix.GetColumn(1));
        }

        /// <summary>
        /// Return the scale of the matrix
        /// </summary>
        public static Vector3 Scale(this Matrix4x4 rMatrix)
        {
            /*
            Vector3 scale;
            scale.x = new Vector4(rMatrix.m00, rMatrix.m10, rMatrix.m20, rMatrix.m30).magnitude;
            scale.y = new Vector4(rMatrix.m01, rMatrix.m11, rMatrix.m21, rMatrix.m31).magnitude;
            scale.z = new Vector4(rMatrix.m02, rMatrix.m12, rMatrix.m22, rMatrix.m32).magnitude;
            */
            return new Vector3(rMatrix.GetColumn(0).magnitude, rMatrix.GetColumn(1).magnitude, rMatrix.GetColumn(2).magnitude);
        }

        private static float[] RootVector = new float[4];
        private static float[] ComVector = new float[4];
        /// <summary>
        /// 视锥体内部算法
        /// </summary>
        /// <param name="InCamera"></param>
        /// <param name="OutPlanes"></param>
        public static void CalculateFrustumPlanes(Camera InCamera, ref Plane[] OutPlanes)
        {
            Matrix4x4 projectionMatrix = InCamera.projectionMatrix;
            Matrix4x4 worldToCameraMatrix = InCamera.worldToCameraMatrix;
            Matrix4x4 worldToProjectionMatrix = projectionMatrix * worldToCameraMatrix;

            RootVector[0] = worldToProjectionMatrix[3, 0];
            RootVector[1] = worldToProjectionMatrix[3, 1];
            RootVector[2] = worldToProjectionMatrix[3, 2];
            RootVector[3] = worldToProjectionMatrix[3, 3];

            ComVector[0] = worldToProjectionMatrix[0, 0];
            ComVector[1] = worldToProjectionMatrix[0, 1];
            ComVector[2] = worldToProjectionMatrix[0, 2];
            ComVector[3] = worldToProjectionMatrix[0, 3];

            CalcPlane(ref OutPlanes[(int)PlaneSide.Left], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
            CalcPlane(ref OutPlanes[(int)PlaneSide.Right], -ComVector[0] + RootVector[0], -ComVector[1] + RootVector[1], -ComVector[2] + RootVector[2], -ComVector[3] + RootVector[3]);

            ComVector[0] = worldToProjectionMatrix[1, 0];
            ComVector[1] = worldToProjectionMatrix[1, 1];
            ComVector[2] = worldToProjectionMatrix[1, 2];
            ComVector[3] = worldToProjectionMatrix[1, 3];

            CalcPlane(ref OutPlanes[(int)PlaneSide.Bottom], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
            CalcPlane(ref OutPlanes[(int)PlaneSide.Top], -ComVector[0] + RootVector[0], -ComVector[1] + RootVector[1], -ComVector[2] + RootVector[2], -ComVector[3] + RootVector[3]);

            ComVector[0] = worldToProjectionMatrix[2, 0];
            ComVector[1] = worldToProjectionMatrix[2, 1];
            ComVector[2] = worldToProjectionMatrix[2, 2];
            ComVector[3] = worldToProjectionMatrix[2, 3];

            CalcPlane(ref OutPlanes[(int)PlaneSide.Near], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
            CalcPlane(ref OutPlanes[(int)PlaneSide.Far], -ComVector[0] + RootVector[0], -ComVector[1] + RootVector[1], -ComVector[2] + RootVector[2], -ComVector[3] + RootVector[3]);
        }

        /// <summary>
        /// 点法式方程
        /// </summary>
        /// <param name="InPlane"></param>
        /// <param name="InA"></param>
        /// <param name="InB"></param>
        /// <param name="InC"></param>
        /// <param name="InDistance"></param>
        static void CalcPlane(ref Plane InPlane, float InA, float InB, float InC, float InDistance)
        {
            Vector3 Normal = new Vector3(InA, InB, InC);
            float InverseMagnitude = 1.0f / Mathf.Sqrt(Normal.x * Normal.x + Normal.y * Normal.y + Normal.z * Normal.z);
            InPlane.normal = new Vector3(Normal.x * InverseMagnitude, Normal.y * InverseMagnitude, Normal.z * InverseMagnitude);
            InPlane.distance = InDistance * InverseMagnitude;
        }
    }
}