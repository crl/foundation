using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [RequireComponent(typeof (Text)), AddComponentMenu("UI/Effects/Depth Effect", 2)]
    public class DepthEffect : BaseVertexEffect
    {
        private Vector2 m_BottomRightPos = Vector2.zero;
        [SerializeField] private Vector2 m_DepthPerspectiveStrength = new Vector2(0f, 0f);
        [SerializeField] private Color m_EffectColor = new Color(0f, 0f, 0f, 1f);
        [SerializeField] private Vector2 m_EffectDirectionAndDepth = new Vector2(-1f, 1f);
        [SerializeField] private bool m_OnlyInitialCharactersGenerateDepth = true;
        private Vector2 m_OverallTextSize = Vector2.zero;
        private Vector2 m_TopLeftPos = Vector2.zero;
        [SerializeField] private bool m_UseGraphicAlpha = true;

        protected DepthEffect()
        {
        }

        protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y,
            float factor)
        {
            for (int i = start; i < end; i++)
            {
                UIVertex item = verts[i];
                verts.Add(item);
                Vector3 position = item.position;
                position.x += x*factor;
                if (this.depthPerspectiveStrength.x != 0f)
                {
                    position.x -= (this.depthPerspectiveStrength.x*
                                   (((position.x - this.m_TopLeftPos.x)/this.m_OverallTextSize.x) - 0.5f))*factor;
                }
                position.y += y*factor;
                if (this.depthPerspectiveStrength.y != 0f)
                {
                    position.y += (this.depthPerspectiveStrength.y*
                                   (((this.m_TopLeftPos.y - position.y)/this.m_OverallTextSize.y) - 0.5f))*factor;
                }
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

        protected override void ModifyVertices(List<UIVertex> verts)
        {

            int count = verts.Count;
            Text component = base.GetComponent<Text>();
            List<UIVertex> range = new List<UIVertex>();
            if (this.m_OnlyInitialCharactersGenerateDepth)
            {
                range = verts.GetRange(verts.Count - (component.cachedTextGenerator.characterCountVisible*6),
                    component.cachedTextGenerator.characterCountVisible*6);
            }
            else
            {
                range = verts;
            }
            if (range.Count != 0)
            {
                if ((this.depthPerspectiveStrength.x != 0f) || (this.depthPerspectiveStrength.y != 0f))
                {
                    UIVertex vertex2 = range[0];
                    this.m_TopLeftPos = vertex2.position;
                    UIVertex vertex3 = range[range.Count - 1];
                    this.m_BottomRightPos = vertex3.position;
                    for (int i = 0; i < range.Count; i++)
                    {
                        UIVertex vertex4 = range[i];
                        if (vertex4.position.x < this.m_TopLeftPos.x)
                        {
                            UIVertex vertex5 = range[i];
                            this.m_TopLeftPos.x = vertex5.position.x;
                        }
                        UIVertex vertex6 = range[i];
                        if (vertex6.position.y > this.m_TopLeftPos.y)
                        {
                            UIVertex vertex7 = range[i];
                            this.m_TopLeftPos.y = vertex7.position.y;
                        }
                        UIVertex vertex8 = range[i];
                        if (vertex8.position.x > this.m_BottomRightPos.x)
                        {
                            UIVertex vertex9 = range[i];
                            this.m_BottomRightPos.x = vertex9.position.x;
                        }
                        UIVertex vertex10 = range[i];
                        if (vertex10.position.y < this.m_BottomRightPos.y)
                        {
                            UIVertex vertex11 = range[i];
                            this.m_BottomRightPos.y = vertex11.position.y;
                        }
                    }
                    this.m_OverallTextSize = new Vector2(this.m_BottomRightPos.x - this.m_TopLeftPos.x,
                        this.m_TopLeftPos.y - this.m_BottomRightPos.y);
                }
                int start = 0;
                int num4 = 0;
                start = num4;
                num4 = range.Count;
                this.ApplyShadowZeroAlloc(range, this.effectColor, start, range.Count,
                    this.effectDirectionAndDepth.x, this.effectDirectionAndDepth.y, 0.25f);
                start = num4;
                num4 = range.Count;
                this.ApplyShadowZeroAlloc(range, this.effectColor, start, range.Count,
                    this.effectDirectionAndDepth.x, this.effectDirectionAndDepth.y, 0.5f);
                start = num4;
                num4 = range.Count;
                this.ApplyShadowZeroAlloc(range, this.effectColor, start, range.Count,
                    this.effectDirectionAndDepth.x, this.effectDirectionAndDepth.y, 0.75f);
                start = num4;
                num4 = range.Count;
                this.ApplyShadowZeroAlloc(range, this.effectColor, start, range.Count,
                    this.effectDirectionAndDepth.x, this.effectDirectionAndDepth.y, 1f);
                if (this.onlyInitialCharactersGenerateDepth)
                {
                    range.RemoveRange(range.Count - (component.cachedTextGenerator.characterCountVisible*6),
                        component.cachedTextGenerator.characterCountVisible*6);
                    range.AddRange(verts);
                }
                if (component.material.shader == Shader.Find("Text Effects/Fancy Text"))
                {
                    for (int j = 0; j < (range.Count - count); j++)
                    {
                        UIVertex vertex = range[j];
                        vertex.uv1 = new Vector2(0f, 0f);
                        range[j] = vertex;
                    }
                }
            }

        }

        public Vector2 depthPerspectiveStrength
        {
            get { return this.m_DepthPerspectiveStrength; }
            set
            {
                if (this.m_DepthPerspectiveStrength != value)
                {
                    this.m_DepthPerspectiveStrength = value;
                    if (base.graphic != null)
                    {
                        base.graphic.SetVerticesDirty();
                    }
                }
            }
        }

        public Color effectColor
        {
            get { return this.m_EffectColor; }
            set
            {
                this.m_EffectColor = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public Vector2 effectDirectionAndDepth
        {
            get { return this.m_EffectDirectionAndDepth; }
            set
            {
                if (this.m_EffectDirectionAndDepth != value)
                {
                    this.m_EffectDirectionAndDepth = value;
                    if (base.graphic != null)
                    {
                        base.graphic.SetVerticesDirty();
                    }
                }
            }
        }

        public bool onlyInitialCharactersGenerateDepth
        {
            get { return this.m_OnlyInitialCharactersGenerateDepth; }
            set
            {
                this.m_OnlyInitialCharactersGenerateDepth = value;
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