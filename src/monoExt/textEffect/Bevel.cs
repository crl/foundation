using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [RequireComponent(typeof (Text)), AddComponentMenu("UI/Effects/Outer Bevel", 4)]
    public class Bevel : BaseVertexEffect
    {
        [SerializeField] private Vector2 m_BevelDirectionAndDepth = new Vector2(1f, 1f);
        [SerializeField] private Color m_HighlightColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color m_ShadowColor = new Color(0f, 0f, 0f, 1f);
        [SerializeField] private bool m_UseGraphicAlpha = true;

        protected Bevel()
        {
        }

        protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            for (int i = start; i < end; i++)
            {
                UIVertex item = verts[i];
                verts.Add(item);
                Vector3 position = item.position;
                position.x += x;
                position.y += y;
                item.position = position;
                Color32 color2 = color;
                if (this.useGraphicAlpha)
                {
                    UIVertex vertex2 = verts[i];
                    color2.a = (byte) ((color2.a*vertex2.color.a)/0xff);
                }
                item.color = color2;
                verts[i] = item;
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
                return;

            List<UIVertex> vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            ModifyVertices(vertexList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }

        protected override void ModifyVertices(List<UIVertex> verts)
        {

            int count = verts.Count;
            int start = 0;
            int num3 = 0;
            start = num3;
            num3 = verts.Count;
            this.ApplyShadowZeroAlloc(verts, this.shadowColor, start, verts.Count,
                this.bevelDirectionAndDepth.x*0.75f, -this.bevelDirectionAndDepth.y*0.75f);
            start = num3;
            num3 = verts.Count;
            this.ApplyShadowZeroAlloc(verts, this.shadowColor, start, verts.Count, this.bevelDirectionAndDepth.x,
                this.bevelDirectionAndDepth.y*0.5f);
            start = num3;
            num3 = verts.Count;
            this.ApplyShadowZeroAlloc(verts, this.shadowColor, start, verts.Count,
                -this.bevelDirectionAndDepth.x*0.5f, -this.bevelDirectionAndDepth.y);
            start = num3;
            num3 = verts.Count;
            this.ApplyShadowZeroAlloc(verts, this.highlightColor, start, verts.Count, -this.bevelDirectionAndDepth.x,
                this.bevelDirectionAndDepth.y*0.5f);
            start = num3;
            num3 = verts.Count;
            this.ApplyShadowZeroAlloc(verts, this.highlightColor, start, verts.Count,
                -this.bevelDirectionAndDepth.x*0.5f, this.bevelDirectionAndDepth.y);
            if (base.GetComponent<Text>().material.shader == Shader.Find("Text Effects/Fancy Text"))
            {
                for (int i = 0; i < (verts.Count - count); i++)
                {
                    UIVertex vertex = verts[i];
                    vertex.uv1 = new Vector2(0f, 0f);
                    verts[i] = vertex;
                }
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

        public bool useGraphicAlpha
        {
            get { return this.m_UseGraphicAlpha; }
            set
            {
                this.m_UseGraphicAlpha = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }
    }

}