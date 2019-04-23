using UnityEngine;

namespace gameSDK
{
    public class LightMapRenderer : MonoBehaviour
    {
        public int lightmapIndex;

        public Vector4 lightmapScaleOffset;

        public void SaveSettings()
        {
            Renderer renderer = GetComponent<Renderer>();
            lightmapIndex = renderer.lightmapIndex;
            lightmapScaleOffset = renderer.lightmapScaleOffset;
        }

        public void LoadSettings()
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.lightmapIndex = lightmapIndex;
            renderer.lightmapScaleOffset = lightmapScaleOffset;
        }

        private void Start()
        {
            if (Application.isPlaying)
                LoadSettings();
        }
    }
}