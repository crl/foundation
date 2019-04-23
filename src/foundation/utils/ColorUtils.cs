using System;
using UnityEngine;

namespace foundation
{
    public class ColorUtils
    {
        public static Color32 ToColor(uint HexVal)
        {
            byte R = (byte)((HexVal >> 24) & 0xFF);
            byte G = (byte)((HexVal >> 16) & 0xFF);
            byte B = (byte)((HexVal>>8) & 0xFF);
            byte A = (byte)((HexVal) & 0xFF);
            return new Color32(R, G, B, A);
        }

        public static Color32 RGBToColor(uint HexVal)
        {
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal >> 8) & 0xFF);
            return new Color32(R, G, B, 0xFF);
        }

        public static string ColorToString(Color color)
        {
            return GetColor16((int)(color.r * 255))
                + GetColor16((int)(color.g * 255))
                + GetColor16((int)(color.b * 255))
                + GetColor16((int)(color.a * 255));
        }

        private static string GetColor16(int a)
        {
            string r = Convert.ToString(a, 16);
            return r.Length < 2 ? "0" + r : r;
        }

        public static Color ReadExternal(IDataInput input)
        {
            return new Color(input.ReadFloat(), input.ReadFloat(), input.ReadFloat(),input.ReadFloat());
        }

        public static void WriteExternal(Color v, IDataOutput output)
        {
            output.WriteFloat(v.r);
            output.WriteFloat(v.g);
            output.WriteFloat(v.b);
            output.WriteFloat(v.a);
        }

    }
}