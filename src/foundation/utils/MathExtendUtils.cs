using System;
using UnityEngine;

namespace foundation
{

    public class MathExtendUtils
    {
        public const float TWO_PI = (float)Math.PI * 2;
        /** Moves a radian angle into the range [-PI, +PI], while keeping the direction intact. */
        public static float NormalizeAngle(float angle)
        {
            // move to equivalent value in range [0 deg, 360 deg] without a loop
            angle = angle % 360f;

            // move to [-180 deg, +180 deg]
            if (angle < -180f) angle += 360f;
            if (angle > 180f) angle -= 360f;

            return angle;
        }

        public const float MIN = 0.01f;
        /// <summary>
        /// 计算范围
        /// </summary>
        /// <param name="TopLeft"></param>
        /// <param name="BottomRight"></param>
        /// <returns></returns>
        public static Bounds CaculateBounds(Vector2 TopLeft, Vector2 BottomRight)
        {
            Vector3[] points = new Vector3[5];
            points[0] = new Vector2(TopLeft.x, TopLeft.y);
            points[1] = new Vector2(BottomRight.x, TopLeft.y);
            points[2] = new Vector2(BottomRight.x, BottomRight.y);
            points[3] = new Vector2(TopLeft.x, BottomRight.y);
            points[4] = new Vector2(TopLeft.x, TopLeft.y);
            float width = Vector2.Distance(points[0], points[1]);
            float height = Vector2.Distance(points[1], points[2]);
            return new Bounds(new Vector2((float)System.Math.Round((points[0].x + points[1].x) / 2, 2), (float)System.Math.Round((points[0].y + points[2].y) / 2, 2)), new Vector2((float)System.Math.Round(width, 2), (float)System.Math.Round(height, 2)));
        }


        public static float Random
        {
            get
            {
                return UnityEngine.Random.Range(0, 10)/10.0f;
            }
        }

        public static long RoundToLong(float v)
        {
            long result = (long)v;
            return v - result > 0.5f ? result + 1 : result;
        }


        /// <summary>
        /// 截取保留小数点后 n 位.
        /// </summary>
        /// <param name='num'>
        /// 原数据 num.
        /// </param>
        /// <param name='n'>
        /// 保留小数点后 n 位.
        /// </param>
        public static float Round(float num, int n)
        {
            float temp = num * Mathf.Pow(10, n);
            return Mathf.Round(temp) / Mathf.Pow(10, n);
        }

        /// <summary>
        /// 判断两个 Vector3 是否相等
        /// </summary>
        /// <param name="pos">Vector3</param>
        /// <param name="n">对比到小数后多少位</param>
        /// <returns></returns>
        public static bool CampareVector3(Vector3 pos1, Vector3 pos2, int n = 2)
        {
            if (MathExtendUtils.Round(pos1.x, n) == MathExtendUtils.Round(pos2.x, n)
                && MathExtendUtils.Round(pos1.y, n) == MathExtendUtils.Round(pos2.y, n)
                && MathExtendUtils.Round(pos1.z, n) == MathExtendUtils.Round(pos2.z, n)
                )
            {
                return true;
            }
            return false;
        }

        static public Rect ConvertToTexCoords(Rect rect, int width, int height)
        {
            Rect final = rect;

            if (width != 0f && height != 0f)
            {
                final.xMin = rect.xMin / width;
                final.xMax = rect.xMax / width;
                final.yMin = 1f - rect.yMax / height;
                final.yMax = 1f - rect.yMin / height;
            }
            return final;
        }

        /// <summary>
        /// 获得 2D 空间两点坐标之间的弧度.
        /// </summary>
        public static float Get2DRadianOfTwoPos(Vector2 startPos, Vector2 targetPos)
        {
            return Mathf.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x);
        }

        /// <summary>
        /// 获得 2D 空间两点坐标之间的夹角.
        /// </summary>
        public static float Get2DAngleOfTwoPos(Vector2 startPos, Vector2 targetPos)
        {
            //	Mathf.Rad2Deg = 360 / (PI * 2) = 180 / PI;
            return Mathf.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x) * Mathf.Rad2Deg;
        }


      
        public static uint BinomialCoefficient(uint n, uint k)
        {
            uint num = 1;
            if (k > n)
            {
                return 0;
            }
            for (uint i = 1; i <= k; i++)
            {
                num *= n;
                n--;
                num /= i;
            }
            return num;
        }

        public static Vector2 CatmullRom(float v, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 at = CatmullRomTangent(p0, p2);
            Vector2 bt = CatmullRomTangent(p1, p3);
            return Hermite(v, p1, at, p2, bt);
        }

        public static Vector3 CatmullRom(float v, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 at = CatmullRomTangent(p0, p2);
            Vector3 bt = CatmullRomTangent(p1, p3);
            return Hermite(v, p1, at, p2, bt);
        }
       

        public static Vector2 Hermite(float v, Vector2 a, Vector2 at, Vector2 b, Vector2 bt)
        {
            float num = v * v;
            float num2 = v * num;
            float num3 = (2f * num2) - (3f * num);
            float num4 = num3 + 1f;
            float num5 = -num3;
            float num6 = (num2 - (2f * num)) + v;
            float num7 = num2 - num;
            return (Vector2)((((a * num4) + (b * num5)) + (at * num6)) + (bt * num7));
        }

        public static Vector3 Hermite(float v, Vector3 a, Vector3 at, Vector3 b, Vector3 bt)
        {
            float num = v * v;
            float num2 = v * num;
            float num3 = (2f * num2) - (3f * num);
            float num4 = num3 + 1f;
            float num5 = -num3;
            float num6 = (num2 - (2f * num)) + v;
            float num7 = num2 - num;
            return (Vector3)((((a * num4) + (b * num5)) + (at * num6)) + (bt * num7));
        }

        public static float InverseClamp(float value, float min, float max)
        {
            float f = min - value;
            float num2 = max - value;
            if ((f >= 0f) || (num2 <= 0f))
            {
                return value;
            }
            return ((Mathf.Abs(f) >= num2) ? max : min);
        }

        public static bool IsInRange<T>(T value, T min, T max) where T : IComparable
        {
            return ((value.CompareTo(min) >= 0) && (value.CompareTo(max) <= 0));
        }

        public static T Loop<T>(T value, T min, T max) where T : IComparable
        {
            if (value.CompareTo(max) > 0)
            {
                return min;
            }
            if (value.CompareTo(min) < 0)
            {
                return max;
            }
            return value;
        }

        public static uint LowerPowerOfTwo(uint v)
        {
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 0x10;
            return (v - (v >> 1));
        }

        public static Matrix4x4 MatrixFromFunction(Func<Vector3, Vector3> linearFunction)
        {
            Matrix4x4 matrixx = new Matrix4x4();
            Vector3 vector = linearFunction(new Vector3(1f, 0f, 0f));
            matrixx.SetColumn(0, new Vector4(vector[0], vector[1], vector[2], 0f));
            Vector3 vector2 = linearFunction(new Vector3(0f, 1f, 0f));
            matrixx.SetColumn(1, new Vector4(vector2[0], vector2[1], vector2[2], 0f));
            Vector3 vector3 = linearFunction(new Vector3(0f, 0f, 1f));
            matrixx.SetColumn(2, new Vector4(vector3[0], vector3[1], vector3[2], 0f));
            matrixx.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
            return matrixx;
        }

        public static Matrix4x4 MatrixFromFunction(Func<Vector4, Vector4> linearFunction)
        {
            Matrix4x4 matrixx = new Matrix4x4();
            matrixx.SetColumn(0, linearFunction(new Vector4(1f, 0f, 0f, 0f)));
            matrixx.SetColumn(1, linearFunction(new Vector4(0f, 1f, 0f, 0f)));
            matrixx.SetColumn(2, linearFunction(new Vector4(0f, 0f, 1f, 0f)));
            matrixx.SetColumn(3, linearFunction(new Vector4(0f, 0f, 0f, 1f)));
            return matrixx;
        }

        public static void MultiplyComponents(ref Vector2 vector, Vector2 multiplier)
        {
            vector.x *= multiplier.x;
            vector.y *= multiplier.y;
        }

        public static Vector2 MultiplyComponents(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public static void MultiplyComponents(ref Vector3 vector, Vector3 multiplier)
        {
            vector.x *= multiplier.x;
            vector.y *= multiplier.y;
            vector.z *= multiplier.z;
        }

        public static Vector3 MultiplyComponents(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static void Normalize(ref Quaternion q)
        {
            float f = (((q.w * q.w) + (q.x * q.x)) + (q.y * q.y)) + (q.z * q.z);
            if ((0.9999f >= f) || (f >= 1.0001f))
            {
                float num2 = Mathf.Sqrt(f);
                q.w /= num2;
                q.x /= num2;
                q.y /= num2;
                q.z /= num2;
            }
        }

        public static Vector2 QuadraticBezier(float v, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float num = 1f - v;
            return (Vector2)((num * ((num * p0) + (v * p1))) + (v * ((num * p1) + (v * p2))));
        }

        public static Vector3 QuadraticBezier(float v, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float num = 1f - v;
            return (Vector3)((num * ((num * p0) + (v * p1))) + (v * ((num * p1) + (v * p2))));
        }

        public static bool QuadraticRoots(float a, float b, float c, ref float root1, ref float root2)
        {
            float f = (b * b) - ((4f * a) * c);
            if (f < 0f)
            {
                return false;
            }
            root1 = (-b - Mathf.Sqrt(f)) / (2f * a);
            root2 = (-b + Mathf.Sqrt(f)) / (2f * a);
            return true;
        }

        public static Vector2 QuarticBezier(float v, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float num = v * v;
            float num2 = num * v;
            float num3 = num2 * v;
            float num4 = num3 * v;
            float num5 = 1f - v;
            float num6 = num5 * num5;
            float num7 = num6 * num5;
            float num8 = num7 * num5;
            float num9 = num8 * num5;
            return (Vector2)((((((num9 * p0) + (((5f * v) * num8) * p1)) + (((10f * num) * num7) * p2)) + (((10f * num2) * num6) * p3)) + (((5f * num3) * num5) * p4)) + (num4 * p4));
        }

        public static Vector3 QuarticBezier(float v, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            float num = v * v;
            float num2 = num * v;
            float num3 = num2 * v;
            float num4 = num3 * v;
            float num5 = 1f - v;
            float num6 = num5 * num5;
            float num7 = num6 * num5;
            float num8 = num7 * num5;
            float num9 = num8 * num5;
            return (Vector3)((((((num9 * p0) + (((5f * v) * num8) * p1)) + (((10f * num) * num7) * p2)) + (((10f * num2) * num6) * p3)) + (((5f * num3) * num5) * p4)) + (num4 * p4));
        }


        public static Vector2 CatmullRomTangent(Vector2 a, Vector2 b)
        {
            return (Vector2)((b - a) * 0.5f);
        }

        public static Vector3 CatmullRomTangent(Vector3 a, Vector3 b)
        {
            return (Vector3)((b - a) * 0.5f);
        }

        public static bool CircleLineIntersections(Vector2 p, Vector2 n, Vector2 q, float r, ref float dist1, ref float dist2)
        {
            float sqrMagnitude = n.sqrMagnitude;
            float b = 2f * Vector3.Dot((Vector3)(p - q), (Vector3)n);
            float c = ((p.sqrMagnitude + q.sqrMagnitude) - (2f * Vector3.Dot((Vector3)q, (Vector3)p))) - (r * r);
            return QuadraticRoots(sqrMagnitude, b, c, ref dist1, ref dist2);
        }

        public static double Clamp(double v, double min, double max)
        {
            if (v < min)
            {
                return min;
            }
            if (v > max)
            {
                return max;
            }
            return v;
        }

        public static long Clamp(long v, long min, long max)
        {
            if (v < min)
            {
                return min;
            }
            if (v > max)
            {
                return max;
            }
            return v;
        }

        public static void Clamp(ref Vector2 source, float min, float max)
        {
            float magnitude = source.magnitude;
            source = (Vector2)(source.normalized * Mathf.Clamp(magnitude, min, max));
        }

        public static void Clamp(ref Vector3 source, float min, float max)
        {
            float magnitude = source.magnitude;
            source = (Vector3)(source.normalized * Mathf.Clamp(magnitude, min, max));
        }

        public static Vector2 Clamp(Vector2 source, float min, float max)
        {
            float magnitude = source.magnitude;
            return (Vector2)(source.normalized * Mathf.Clamp(magnitude, min, max));
        }

        public static Vector3 Clamp(Vector3 source, float min, float max)
        {
            float magnitude = source.magnitude;
            return (Vector3)(source.normalized * Mathf.Clamp(magnitude, min, max));
        }

        public static float Clamp01(double v)
        {
            return (float)Clamp(v, 0.0, 1.0);
        }

        public static void ClampComponents(ref Vector2 source, float min, float max)
        {
            source.x = Mathf.Clamp(source.x, min, max);
            source.y = Mathf.Clamp(source.y, min, max);
        }

        public static void ClampComponents(ref Vector3 source, float min, float max)
        {
            source.x = Mathf.Clamp(source.x, min, max);
            source.y = Mathf.Clamp(source.y, min, max);
            source.z = Mathf.Clamp(source.z, min, max);
        }

        public static Vector2 ClampComponents(Vector2 source, float min, float max)
        {
            source.x = Mathf.Clamp(source.x, min, max);
            source.y = Mathf.Clamp(source.y, min, max);
            return source;
        }

        public static Vector3 ClampComponents(Vector3 source, float min, float max)
        {
            source.x = Mathf.Clamp(source.x, min, max);
            source.y = Mathf.Clamp(source.y, min, max);
            source.z = Mathf.Clamp(source.z, min, max);
            return source;
        }

        public static void ClampComponents01(ref Vector2 source)
        {
            source.x = Mathf.Clamp(source.x, 0f, 1f);
            source.y = Mathf.Clamp(source.y, 0f, 1f);
        }

        public static void ClampComponents01(ref Vector3 source)
        {
            source.x = Mathf.Clamp(source.x, 0f, 1f);
            source.y = Mathf.Clamp(source.y, 0f, 1f);
            source.z = Mathf.Clamp(source.z, 0f, 1f);
        }

        public static Vector2 ClampComponents01(Vector2 source)
        {
            source.x = Mathf.Clamp(source.x, 0f, 1f);
            source.y = Mathf.Clamp(source.y, 0f, 1f);
            return source;
        }

        public static Vector3 ClampComponents01(Vector3 source)
        {
            source.x = Mathf.Clamp(source.x, 0f, 1f);
            source.y = Mathf.Clamp(source.y, 0f, 1f);
            source.z = Mathf.Clamp(source.z, 0f, 1f);
            return source;
        }
        public static Vector2 CubicBezier(float v, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float num = 1f - v;
            float num2 = num * num;
            float num3 = v * v;
            return (Vector2)(((((num2 * num) * p0) + (((3f * num2) * v) * p1)) + (((3f * num) * num3) * p2)) + ((num3 * v) * p3));
        }

        public static Vector3 CubicBezier(float v, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float num = 1f - v;
            float num2 = num * num;
            float num3 = v * v;
            return (Vector3)(((((num2 * num) * p0) + (((3f * num2) * v) * p1)) + (((3f * num) * num3) * p2)) + ((num3 * v) * p3));
        }

    }


}
