using UnityEngine;

namespace foundation
{
    [System.Serializable]
    public class DayNightParms
    {
        public int hour;
        [Range(0.0f, 1.5f)] public float lightPower = 1.0f;
        public Color mainLightColor = Color.white;
        public Color specularColor = Color.white;
        public Color ambientColor = Color.gray;
        public Color fogColor = Color.gray;

        [Range(0.0f, 1.5f)] public float bloomThreshold = 0.25f;
    }
}