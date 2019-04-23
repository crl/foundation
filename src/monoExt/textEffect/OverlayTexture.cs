using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [RequireComponent(typeof (Text)), AddComponentMenu("UI/Effects/Overlay Texture", 0x12),
     DisallowMultipleComponent]
    public class OverlayTexture : BaseVertexEffect, IMaterialModifier
    {
        [SerializeField] private ColorMode m_ColorMode;
        private bool m_NeedsToSetMaterialDirty;
        [SerializeField] public Texture2D m_OverlayTexture;
        [SerializeField] private TextureMode m_TextureMode;

        protected OverlayTexture()
        {
        }

        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!this.IsActive())
            {
                return baseMaterial;
            }
            if (baseMaterial.shader != Shader.Find("Text Effects/Fancy Text"))
            {
                Debug.Log("\"" + base.gameObject.name +
                          "\" doesn't have the \"Fancy Text\" shader applied. Please use it if you are using the \"Overlay Texture\" effect.");
                return baseMaterial;
            }
            Material material = new Material(baseMaterial)
            {
                name = baseMaterial.name + " with OT",
                hideFlags = HideFlags.HideAndDontSave
            };
            material.shaderKeywords = baseMaterial.shaderKeywords;
            material.CopyPropertiesFromMaterial(baseMaterial);
            material.EnableKeyword("_USEOVERLAYTEXTURE_ON");
            material.SetTexture("_OverlayTex", this.overlayTexture);
            material.SetInt("_OverlayTextureColorMode", (int) this.colorMode);
            this.m_NeedsToSetMaterialDirty = true;
            return material;
        }

        protected override void ModifyVertices(List<UIVertex> verts)
        {
            int count = verts.Count;
            if (verts.Count != 0)
            {
                UIVertex vertex;
                if (this.textureMode == TextureMode.Global)
                {
                    UIVertex vertex2 = verts[0];
                    Vector2 position = vertex2.position;
                    UIVertex vertex3 = verts[verts.Count - 1];
                    Vector2 vector2 = vertex3.position;
                    for (int i = 0; i < verts.Count; i++)
                    {
                        UIVertex vertex4 = verts[i];
                        if (vertex4.position.x < position.x)
                        {
                            UIVertex vertex5 = verts[i];
                            position.x = vertex5.position.x;
                        }
                        UIVertex vertex6 = verts[i];
                        if (vertex6.position.y > position.y)
                        {
                            UIVertex vertex7 = verts[i];
                            position.y = vertex7.position.y;
                        }
                        UIVertex vertex8 = verts[i];
                        if (vertex8.position.x > vector2.x)
                        {
                            UIVertex vertex9 = verts[i];
                            vector2.x = vertex9.position.x;
                        }
                        UIVertex vertex10 = verts[i];
                        if (vertex10.position.y < vector2.y)
                        {
                            UIVertex vertex11 = verts[i];
                            vector2.y = vertex11.position.y;
                        }
                    }
                    float num3 = position.y - vector2.y;
                    float num4 = vector2.x - position.x;
                    for (int j = 0; j < count; j++)
                    {
                        vertex = verts[j];
                        vertex.uv1 = new Vector2(1f + ((vertex.position.x - position.x)/num4),
                            2f - ((position.y - vertex.position.y)/num3));
                        verts[j] = vertex;
                    }
                }
                else
                {
                    for (int k = 0; k < count; k++)
                    {
                        vertex = verts[k];
                        vertex.uv1 =
                            new Vector2((float) (1 + (((((k%6) != 0) && ((k%6) != 5)) && ((k%6) != 4)) ? 1 : 0)),
                                (float) (1 + (((((k%6) != 2) && ((k%6) != 3)) && ((k%6) != 4)) ? 1 : 0)));
                        verts[k] = vertex;
                    }
                }
            }

        }

        protected override void Start()
        {
            if (base.graphic != null)
            {
                base.graphic.SetMaterialDirty();
            }
        }

        private void Update()
        {
            if (this.m_NeedsToSetMaterialDirty && (base.graphic != null))
            {
                base.graphic.SetMaterialDirty();
            }
        }

        public ColorMode colorMode
        {
            get { return this.m_ColorMode; }
            set
            {
                this.m_ColorMode = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public Texture2D overlayTexture
        {
            get { return this.m_OverlayTexture; }
            set
            {
                this.m_OverlayTexture = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public TextureMode textureMode
        {
            get { return this.m_TextureMode; }
            set
            {
                this.m_TextureMode = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public enum ColorMode
        {
            Override,
            Additive,
            Multiply
        }

        public enum TextureMode
        {
            Local,
            Global
        }
    }
}
