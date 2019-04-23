using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [AddComponentMenu("UI/Effects/Inner Outline", 20), DisallowMultipleComponent, RequireComponent(typeof (Text))]
    public class InnerOutline : BaseVertexEffect, IMaterialModifier
    {
        [SerializeField] private ColorMode m_ColorMode;
        private bool m_NeedsToSetMaterialDirty;
        [SerializeField] public Color m_OutlineColor = Color.black;
        [SerializeField] private float m_OutlineThickness = 1f;

        protected InnerOutline()
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
                          "\" doesn't have the \"Fancy Text\" shader applied. Please use it if you are using the \"Inner Outline\" effect.");
                return baseMaterial;
            }
            Material material = new Material(baseMaterial)
            {
                name = baseMaterial.name + " with IO",
                hideFlags = HideFlags.HideAndDontSave
            };
            material.shaderKeywords = baseMaterial.shaderKeywords;
            material.CopyPropertiesFromMaterial(baseMaterial);
            material.EnableKeyword("_USEOUTLINE_ON");
            material.SetColor("_OutlineColor", this.outlineColor);
            material.SetFloat("_OutlineThickness", this.outlineThickness/250f);
            material.SetInt("_OutlineColorMode", (int) this.colorMode);
            this.m_NeedsToSetMaterialDirty = true;
            return material;
        }

        protected override void ModifyVertices(List<UIVertex> verts)
        {
            int count = verts.Count;
            for (int i = 0; i < count; i++)
            {
                UIVertex vertex = verts[i];
                if (vertex.uv1 == Vector2.zero)
                {
                    vertex.uv1 = new Vector2(1f, 1f);
                }
                verts[i] = vertex;
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

        public Color outlineColor
        {
            get { return this.m_OutlineColor; }
            set
            {
                this.m_OutlineColor = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public float outlineThickness
        {
            get { return this.m_OutlineThickness; }
            set
            {
                this.m_OutlineThickness = value;
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