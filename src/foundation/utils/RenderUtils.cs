using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace foundation
{
    public enum BlendType
    {
        Opaque,
        Cutout,
        Fade,       // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
    }
    public class RenderUtils
    {
        private static string defaultShaderName = "Standard";
        private static ASDictionary<string, Shader> shaderCache=new ASDictionary<Shader>();
        public static bool DEBUG = true;
        public static bool DEBUG_ERROR_USE_STAND = true;
        public static void ShaderFind(GameObject go)
        {
            if (go == null || Application.isMobilePlatform)
            {
                return;
            }

            Renderer[] meshRenderers = go.GetComponentsInChildren<Renderer>(true);
            int len = meshRenderers.Length;
            for (int i = 0; i < len; i++)
            {
                Renderer meshRenderer = meshRenderers[i];
                Material[] sharedMaterials = meshRenderer.sharedMaterials;
                int mlen = sharedMaterials.Length;
                for (int j = 0; j < mlen; j++)
                {
                    Material material = sharedMaterials[j];
                    RebindMaterial(material, meshRenderer.gameObject);
                }
            }

            Image[] images = go.GetComponentsInChildren<Image>(true);
            len = images.Length;
            for (int i = 0; i < len; i++)
            {
                RebindMaterial(images[i].material,go);
            }
        }


        public static Shader FindShader(string shaderName, bool cache = true)
        {
            Shader shader=shaderCache[shaderName];
            if (shader == null)
            {
                shader= Shader.Find(shaderName);
                if (cache)
                {
                    shaderCache[shaderName] = shader;
                }
            }
            return shader;
        }

        public static void ClearAllCacheShader()
        {
            shaderCache.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <param name="owner">材质的拥有者</param>
        public static void RebindMaterial(Material material,GameObject owner=null)
        {
            if (material == null || Application.isMobilePlatform)
            {
                return;
            }
            Shader shader = material.shader;
            if (shader == null)
            {
                return;
            }

            string shaderName = shader.name;
            if (Application.isEditor && DEBUG)
            {
                Shader tempShader = Shader.Find(shaderName);
                if (tempShader == null)
                {
                    Debug.LogWarning("shaderName:" + shaderName + " not found!!",owner);
                    if (DEBUG_ERROR_USE_STAND)
                    {
                        shaderName = "Standard";
                        tempShader = Shader.Find(shaderName);
                    }
                }
                material.shader = tempShader;
                return;
            }

            if (shader.isSupported == false)
            {
                Debug.LogWarning("shaderName:" + shaderName + " is not supported!!",owner);
            }
        }

        public static string GetDefaultShaderName()
        {
            return defaultShaderName;
        }
        public static void SetEnabledRecursive(GameObject go, bool enabled)
        {
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {
                if (renderer.enabled != enabled)
                {
                    renderer.enabled = enabled;
                }
            }
        }

        public static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
            {
                m.EnableKeyword(keyword);
            }
            else
            {
                m.DisableKeyword(keyword);
            }
        }

        public static void SetCull(Material m,CullMode cullMode)
        {
            m.SetInt("_Cull",(int)cullMode);
        }

        public static void SetupMaterialWithBlendMode(Material material, BlendType blendMode)
        {
            switch (blendMode)
            {
                case BlendType.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int) BlendMode.One);
                    material.SetInt("_DstBlend", (int) BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case BlendType.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int) BlendMode.One);
                    material.SetInt("_DstBlend", (int) BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case BlendType.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int) BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case BlendType.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int) BlendMode.One);
                    material.SetInt("_DstBlend", (int) BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }

        public static Vector3 GetBoundSize(GameObject go,out Bounds outBounds)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            int len = renderers.Length;
            if (len > 0)
            {
                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
                for (int i = 0; i < len; i++)
                {
                    Renderer r = renderers[i];
                    bounds.Encapsulate(r.bounds);
                }
                outBounds = bounds;
                return bounds.size;
            }
            outBounds = new Bounds();
            return Vector3.zero;
        }

        public static Bounds GetBound(GameObject go)
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            int len = renderers.Length;
            if (len > 0)
            {
                for (int i = 0; i < len; i++)
                {
                    Renderer r = renderers[i];
                    bounds.Encapsulate(r.bounds);
                }
                return bounds;
            }
            return bounds;
        }
    }
}