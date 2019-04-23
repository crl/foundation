using UnityEngine;
using UnityEngine.Rendering;

namespace foundation.monoExt
{
    /// <summary>
    /// 渲染6面体
    /// </summary>
    public class RenderToCubemap:MonoBehaviour
    {
        public int cubemapSize = 128;
        public bool oneFacePerFrame = false;
        private Camera cam ;
        private RenderTexture rtex ;

        protected void Start()
        {
            // render all six faces at startup
            //在启动时渲染所有六个面
            UpdateCubemap(63);
        }

        protected void LateUpdate()
        {
            if (oneFacePerFrame)
            {
                var faceToRender = Time.frameCount % 6;
                var faceMask = 1 << faceToRender;
                UpdateCubemap(faceMask);
            }
            else {
                UpdateCubemap(63); // all six faces 所有六个面
            }
        }

        protected void UpdateCubemap(int faceMask)
        {
            if (!cam)
            {
                GameObject go = new GameObject("CubemapCamera");
                go.hideFlags = HideFlags.HideAndDontSave;
                go.transform.position = transform.position;
                go.transform.rotation = Quaternion.identity;
                cam = go.AddComponent<Camera>();
                cam.farClipPlane = 100; // don't render very far into cubemap //不要渲染较远的部分
                cam.enabled = false;
            }

            if (!rtex)
            {
                rtex = new RenderTexture(cubemapSize, cubemapSize, 16);
                rtex.isPowerOfTwo = true;
                rtex.dimension = TextureDimension.Cube;
                rtex.hideFlags = HideFlags.HideAndDontSave;
                Renderer renderer = GetComponent<Renderer>();
                renderer.sharedMaterial.SetTexture("_Cube", rtex);
            }

            cam.transform.position = transform.position;
            cam.RenderToCubemap(rtex, faceMask);
        }

        protected void OnDisable()
        {
            DestroyImmediate(cam);
            DestroyImmediate(rtex);
        }
    }
}