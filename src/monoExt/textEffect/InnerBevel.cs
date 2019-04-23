using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [AddComponentMenu("UI/Effects/Inner Bevel", 0x13), RequireComponent(typeof (Text)), DisallowMultipleComponent]
    public class InnerBevel : BaseVertexEffect, IMaterialModifier
    {
        [SerializeField] private Vector2 m_BevelDirectionAndDepth = new Vector2(1f, 1f);
        [SerializeField] public Color m_HighlightColor = Color.white;
        [SerializeField] private ColorMode m_HighlightColorMode;
        private bool m_NeedsToSetMaterialDirty;
        [SerializeField] public Color m_ShadowColor = Color.black;
        [SerializeField] private ColorMode m_ShadowColorMode;

        protected InnerBevel()
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
                          "\" doesn't have the \"Fancy Text\" shader applied. Please use it if you are using the \"Inner Bevel\" effect.");
                return baseMaterial;
            }
            Material material = new Material(baseMaterial)
            {
                name = baseMaterial.name + " with IB",
                hideFlags = HideFlags.HideAndDontSave
            };
            material.shaderKeywords = baseMaterial.shaderKeywords;
            material.CopyPropertiesFromMaterial(baseMaterial);
            material.EnableKeyword("_USEBEVEL_ON");
            material.SetColor("_HighlightColor", this.highlightColor);
            material.SetColor("_ShadowColor", this.shadowColor);
            material.SetVector("_HighlightOffset", (Vector4) (this.bevelDirectionAndDepth/500f));
            material.SetInt("_HighlightColorMode", (int) this.highlightColorMode);
            material.SetInt("_ShadowColorMode", (int) this.shadowColorMode);
            this.m_NeedsToSetMaterialDirty = true;
            return material;
        }

        protected override void ModifyVertices(List<UIVertex> verts)
        {

            int count = verts.Count;
            for (int i = 0; i < count; i += 6)
            {
                UIVertex vertex = verts[i];
                UIVertex vertex2 = verts[i + 1];
                UIVertex vertex3 = verts[i];
                Vector2 vector4 = vertex2.uv0 - vertex3.uv0;
                Vector2 normalized = vector4.normalized;
                UIVertex vertex4 = verts[i + 1];
                UIVertex vertex5 = verts[i + 2];
                Vector2 vector5 = vertex4.uv0 - vertex5.uv0;
                Vector2 vector2 = vector5.normalized;
                Vector4 vector3 = normalized;
                vector3.z = vector2.x;
                vector3.w = vector2.y;
                vertex.tangent = vector3;
                if (vertex.uv1 == Vector2.zero)
                {
                    vertex.uv1 = new Vector2(1f, 1f);
                }
                verts[i] = vertex;
                for (int j = 1; j < 6; j++)
                {
                    vertex = verts[i + j];
                    UIVertex vertex6 = verts[i];
                    vertex.tangent = vertex6.tangent;
                    if (vertex.uv1 == Vector2.zero)
                    {
                        vertex.uv1 = new Vector2(1f, 1f);
                    }
                    verts[i + j] = vertex;
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

        public Vector2 bevelDirectionAndDepth
        {
            get { return this.m_BevelDirectionAndDepth; }
            set
            {
                if (this.m_BevelDirectionAndDepth != value)
                {
                    this.m_BevelDirectionAndDepth = value;
                    if (base.graphic != null)
                    {
                        base.graphic.SetVerticesDirty();
                    }
                }
            }
        }

        public Color highlightColor
        {
            get { return this.m_HighlightColor; }
            set
            {
                this.m_HighlightColor = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public ColorMode highlightColorMode
        {
            get { return this.m_HighlightColorMode; }
            set
            {
                this.m_HighlightColorMode = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public Color shadowColor
        {
            get { return this.m_ShadowColor; }
            set
            {
                this.m_ShadowColor = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public ColorMode shadowColorMode
        {
            get { return this.m_ShadowColorMode; }
            set
            {
                this.m_ShadowColorMode = value;
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
    }

}