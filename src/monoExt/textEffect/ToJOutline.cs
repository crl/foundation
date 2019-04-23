using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [AddComponentMenu("UI/Effects/ToJ Outline", 15)]
    public class ToJOutline : Shadow
    {
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
                return;

            List<UIVertex> vertexList = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(vertexList);

            ModifyVertices(vertexList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);

            ListPool<UIVertex>.Release(vertexList);
        }
        new protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            int num = verts.Count*2;
            if (verts.Capacity < num)
            {
                verts.Capacity = num;
            }
            for (int i = start; i < end; i++)
            {
                UIVertex item = verts[i];
                verts.Add(item);
                Vector3 position = item.position;
                position.x += x;
                position.y += y;
                item.position = position;
                Color32 color2 = color;
                if (base.useGraphicAlpha)
                {
                    UIVertex vertex2 = verts[i];
                    color2.a = (byte) ((color2.a*vertex2.color.a)/0xff);
                }
                item.color = color2;
                verts[i] = item;
            }
        }

        protected void ModifyVertices(List<UIVertex> verts)
        {
            int count = verts.Count;
            int num2 = verts.Count*5;
            if (verts.Capacity < num2)
            {
                verts.Capacity = num2;
            }
            int start = 0;
            int num4 = verts.Count;
            this.ApplyShadowZeroAlloc(verts, base.effectColor, start, verts.Count, base.effectDistance.x,
                base.effectDistance.y);
            start = num4;
            num4 = verts.Count;
            this.ApplyShadowZeroAlloc(verts, base.effectColor, start, verts.Count, base.effectDistance.x,
                -base.effectDistance.y);
            start = num4;
            num4 = verts.Count;
            this.ApplyShadowZeroAlloc(verts, base.effectColor, start, verts.Count, -base.effectDistance.x,
                base.effectDistance.y);
            start = num4;
            num4 = verts.Count;
            this.ApplyShadowZeroAlloc(verts, base.effectColor, start, verts.Count, -base.effectDistance.x,
                -base.effectDistance.y);
            Text component = base.GetComponent<Text>();
            if ((component != null) && (component.material.shader == Shader.Find("Text Effects/Fancy Text")))
            {
                for (int i = 0; i < (verts.Count - count); i++)
                {
                    UIVertex vertex = verts[i];
                    vertex.uv1 = new Vector2(0f, 0f);
                    verts[i] = vertex;
                }
            }

        }
    }

}