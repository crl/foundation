using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class ShaderReplacer
    {
        internal static Dictionary<GameObject, ShaderReplaceItem> runing = new Dictionary<GameObject, ShaderReplaceItem>();
        internal static Dictionary<string, string> replaceMap = new Dictionary<string, string>();
        public static bool enabled = true;

        public static void AddMapping(string shaderName, string replaceShaderName)
        {
            replaceMap[shaderName] = replaceShaderName;
        }

        private static bool _hasBorder = true;

        public static bool hasBorder
        {
            set
            {
                if (_hasBorder == value)
                {
                    return;
                }
                _hasBorder = value;

                if (enabled)
                {
                    foreach (ShaderReplaceItem go in runing.Values)
                    {
                        go.replaceBy(_hasBorder);
                    }
                }
            }
        }

        public static void Check(GameObject go)
        {
            if (enabled == false)
            {
                return;
            }
            ShaderReplaceItem shaderReplaceItem = go.GetComponent<ShaderReplaceItem>();
            if (shaderReplaceItem == null)
            {
                shaderReplaceItem = go.AddComponent<ShaderReplaceItem>();
                runing[go] = shaderReplaceItem;
            }
            shaderReplaceItem.replaceBy(_hasBorder);
        }
    }

    public class MaterialRef
    {
        internal Material material;
        internal Shader orgiShader;
        private Shader newShader;

        public void replaceBy(bool _hasBorder)
        {
            string newShaderName = "";
            if (ShaderReplacer.replaceMap.TryGetValue(orgiShader.name, out newShaderName) == false)
            {
                return;
            }

            Shader shader = orgiShader;
            if (_hasBorder == false)
            {
                if (newShader == null)
                {
                    newShader = Shader.Find(newShaderName);
                }
                if (newShader != null)
                {
                    shader = newShader;
                }
            }

            if (shader != material.shader)
            {
                material.shader = shader;
            }
        }
    }

    public class ShaderReplaceItem : MonoBehaviour
    {
        private List<MaterialRef> materialRefList;

        public ShaderReplaceItem()
        {
            materialRefList = new List<MaterialRef>();
        }

        private void Awake()
        {
            materialRefList.Clear();
            Renderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<Renderer>(true);
            int len = skinnedMeshRenderers.Length;
            for (int i = 0; i < len; i++)
            {
                Renderer meshRenderer = skinnedMeshRenderers[i];
                Material[] materials = meshRenderer.sharedMaterials;
                int mlen = materials.Length;
                for (int j = 0; j < mlen; j++)
                {
                    Material material = materials[j];
                    if (material == null)
                    {
                        continue;
                    }
                    Shader shader = material.shader;
                    if (shader == null)
                    {
                        continue;
                    }
                    MaterialRef item = new MaterialRef();
                    item.material = material;
                    item.orgiShader = shader;
                    materialRefList.Add(item);
                }
            }
        }

        protected void OnDestory()
        {
            if (ShaderReplacer.runing.ContainsKey(this.gameObject))
            {
                ShaderReplacer.runing.Remove(this.gameObject);
            }
        }

        public void replaceBy(bool _hasBorder)
        {
            foreach (MaterialRef materialRef in materialRefList)
            {
                materialRef.replaceBy(_hasBorder);
            }
        }
    }
}