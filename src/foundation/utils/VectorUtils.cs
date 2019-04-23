using System;
using UnityEngine;

namespace foundation
{
    public class VectorUtils
    {
        public const float smallestNumber = 0.0000000000000000000001f;

        public static Vector3 sharePosition=Vector3.zero;
        public static Quaternion shareRotation =Quaternion.identity;
        public static Vector3 shareScale =Vector3.one;


        /// <summary>
        /// 是否可见
        /// </summary>
        /// <param name="postion">主对像坐标</param>
        /// <param name="forward">主对像前方向</param>
        /// <param name="fov">主对像视角</param>
        /// <param name="radius">主对像视距</param>
        /// <param name="target">被检测对像坐标</param>
        /// <returns></returns>
        public static float IsVisible(Vector3 postion, Vector3 forward, float fov, float radius, Vector3 target)
        {
            postion.y = 0;
            target.y = 0;

            float angle = Vector3.Angle(forward, target - postion);
            if (Mathf.Abs(angle) < fov * 0.5f)
            {
                float distance = Vector3.Distance(postion, target);
                if (distance <= radius)
                {
                    return distance;
                }
            }
            // If we got here, the target isn't in our FOV or is blocked
            return 0f;
        }


        public static float Distance(Vector3 to, Vector3 from, bool ignoreY = true)
        {
            return Mathf.Sqrt(DistanceSquared(to, from, ignoreY));
        }

        public static Vector3 ReadExternal(IDataInput input)
        {
            return new Vector3(input.ReadFloat(),input.ReadFloat(),input.ReadFloat());
        }

        public static void WriteExternal(Vector3 v,IDataOutput output)
        {
            output.WriteFloat(v.x);
            output.WriteFloat(v.y);
            output.WriteFloat(v.z);
        }

        public static Bounds ReadExternalBound(IDataInput input)
        {
            Vector3 c = new Vector3(input.ReadFloat(), input.ReadFloat(), input.ReadFloat());
            Vector3 e = new Vector3(input.ReadFloat(), input.ReadFloat(), input.ReadFloat());
            return new Bounds(c,e);
        }

        public static void WriteExternal(Bounds v, IDataOutput output)
        {
            Vector3 c = v.center;
            Vector3 e = v.extents;
            output.WriteFloat(c.x);
            output.WriteFloat(c.y);
            output.WriteFloat(c.z);

            output.WriteFloat(e.x);
            output.WriteFloat(e.y);
            output.WriteFloat(e.z);
        }

        public static float DistanceSquared(Vector3 to, Vector3 from, bool ignoreY = true)
        {
            float dx = to.x - from.x;
            float dz = to.z - from.z;

            if (ignoreY)
            {
                return dx*dx + dz*dz;
            }

            float dy = to.y - from.y;
            return dx*dx + dy*dy + dz*dz;
        }

        public static float GetAngleY(Vector3 to, Vector3 from)
        {
            float dx = to.x - from.x;
            float dz = to.z - from.z;
            if (dz == 0)
            {
                dz = smallestNumber;
            }
            float angle = Mathf.Atan2(dx, dz)*Mathf.Rad2Deg;
            return angle;
        }

        public static Vector3 GetPosNearByLen(Vector3 to, Vector3 from, float len, bool canStop = true)
        {
            float distance = Vector3.Distance(to, from);

            if (canStop && distance < len)
            {
                return from;
            }

            Vector3 dir = to - from;
            dir.Normalize();
            return dir*(distance - len) + from;
        }

        public static bool IsNearByLen(Vector3 to, Vector3 from, float len = 0.01f, bool ignoreY = true)
        {
            return DistanceSquared(to, from, ignoreY) < len*len;
        }

        protected static Vector3 GetNearestBy(Vector3 position, int rotationY, float len = 1)
        {
            return (Quaternion.Euler(0, rotationY, 0)*Vector3.forward)*len;
        }

        private static float Q = 360f;

        public static float SmoothAngle(float fromAngle, float toAngle, float deltaTime)
        {
            float delta = toAngle - fromAngle;
            delta = delta - Mathf.FloorToInt(delta/Q)*Q;
            if (delta > 180f)
            {
                delta -= 360;
            }
            return Mathf.Lerp(fromAngle, fromAngle + delta, deltaTime);
        }

        public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
        {
            Matrix4x4 ret = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                ret[i] = Mathf.Lerp(from[i], to[i], time);
            return ret;
        }

        public static Vector3[] CalcCubeVertex(Vector3 center, Vector3 extents)
        {
            Vector3 v3FrontTopLeft = new Vector3(center.x - extents.x, center.y + extents.y,
                center.z - extents.z);
            Vector3 v3FrontTopRight = new Vector3(center.x + extents.x, center.y + extents.y,
                center.z - extents.z);
            Vector3 v3FrontBottomLeft = new Vector3(center.x - extents.x, center.y - extents.y,
                center.z - extents.z);
            Vector3 v3FrontBottomRight = new Vector3(center.x + extents.x, center.y - extents.y,
                center.z - extents.z);
            Vector3 v3BackTopLeft = new Vector3(center.x - extents.x, center.y + extents.y,
                center.z + extents.z); // Back top left corner
            Vector3 v3BackTopRight = new Vector3(center.x + extents.x, center.y + extents.y,
                center.z + extents.z);
            Vector3 v3BackBottomLeft = new Vector3(center.x - extents.x, center.y - extents.y,
                center.z + extents.z);
            Vector3 v3BackBottomRight = new Vector3(center.x + extents.x, center.y - extents.y,
                center.z + extents.z);

            Vector3[] list = new Vector3[24];
            list[0] = (v3FrontTopLeft);
            list[1] = (v3FrontTopRight);
            list[2] = (v3FrontTopRight);
            list[3] = (v3FrontBottomRight);
            list[4] = (v3FrontBottomRight);
            list[5] = (v3FrontBottomLeft);
            list[6] = (v3FrontBottomLeft);
            list[7] = (v3FrontTopLeft);
            list[8] = (v3BackTopLeft);
            list[9] = (v3BackTopRight);
            list[10] = (v3BackTopRight);
            list[11] = (v3BackBottomRight);
            list[12] = (v3BackBottomRight);
            list[13] = (v3BackBottomLeft);
            list[14] = (v3BackBottomLeft);
            list[15] = (v3BackTopLeft);
            list[16] = (v3FrontTopLeft);
            list[17] = (v3BackTopLeft);
            list[18] = (v3FrontTopRight);
            list[19] = (v3BackTopRight);
            list[20] = (v3FrontBottomRight);
            list[21] = (v3BackBottomRight);
            list[22] = (v3FrontBottomLeft);
            list[23] = (v3BackBottomLeft);
            return list;
        }


        public static void Decompose(Matrix4x4 m, out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            if (m == Matrix4x4.zero)
            {
                position=Vector3.zero;
                rotation=Quaternion.identity;
                scale=Vector3.one;
                return;
            }
            position = m.GetColumn(3);
            // Extract new local rotation
            rotation = Quaternion.LookRotation(
                m.GetColumn(2),
                m.GetColumn(1)
                );
            // Extract new local scale
            scale = new Vector3(
                m.GetColumn(0).magnitude,
                m.GetColumn(1).magnitude,
                m.GetColumn(2).magnitude
                );
        }

        /// <summary>
        /// 取一个最近的可走点
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        /// <param name="outPos"></param>
        /// <returns></returns>
        public static bool getNavPosNear(Vector3 position, Vector3 value, out Vector3 outPos)
        {
            outPos = position;
            Vector3 tempPos = value;

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.Raycast(position, tempPos, out hit, UnityEngine.AI.NavMesh.AllAreas) == true)
            {
                outPos = hit.position;
                return true;
            }

            tempPos.y = position.y;
            for (int i = 0; i < 50; i++)
            {
                if (UnityEngine.AI.NavMesh.SamplePosition(tempPos + Vector3.up * 0.2f * i, out hit, 0.3f, UnityEngine.AI.NavMesh.AllAreas) == true)
                {
                    outPos = hit.position;
                    return true;
                }
                if (UnityEngine.AI.NavMesh.SamplePosition(tempPos + Vector3.down * 0.2f * i, out hit, 0.3f, UnityEngine.AI.NavMesh.AllAreas) == true)
                {
                    outPos = hit.position;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 取目标点是否可走
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        /// <param name="outPos"></param>
        /// <returns></returns>
        public static bool getNavPos(Vector3 position, Vector3 value, out Vector3 outPos)
        {
            outPos = Vector3.zero;
            Vector3 tempPos = value;

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.Raycast(position, tempPos, out hit, UnityEngine.AI.NavMesh.AllAreas) == true)
            {
                outPos = hit.position;
                return false;
            }
            tempPos.y = position.y;
            for (int i = 0; i < 50; i++)
            {
                if (UnityEngine.AI.NavMesh.SamplePosition(tempPos + Vector3.up * 0.2f * i, out hit, 0.3f,
                        UnityEngine.AI.NavMesh.AllAreas) == true)
                {
                    outPos = hit.position;
                    return true;
                }
                if (UnityEngine.AI.NavMesh.SamplePosition(tempPos + Vector3.down * 0.2f * i, out hit, 0.3f, UnityEngine.AI.NavMesh.AllAreas) == true)
                {
                    outPos = hit.position;
                    return true;
                }
            }

            return false;
        }
    }
}