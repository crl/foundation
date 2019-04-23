using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [AddComponentMenu("UI/Effects/Gradient Color", 1), RequireComponent(typeof (Text))]
    public class GradientColor : BaseVertexEffect
    {
        [SerializeField] private ColorMode m_ColorMode;
        [SerializeField] public Color m_FirstColor = Color.white;
        [SerializeField] private GradientDirection m_GradientDirection;
        [SerializeField] private GradientMode m_GradientMode;
        [SerializeField] public Color m_SecondColor = Color.black;
        [SerializeField] private bool m_UseGraphicAlpha = true;

        protected GradientColor()
        {
        }

        private Color CalculateColor(Color initialColor, Color newColor, ColorMode colorMode)
        {
            if (colorMode != ColorMode.Override)
            {
                if (colorMode == ColorMode.Additive)
                {
                    return (initialColor + newColor);
                }
                if (colorMode == ColorMode.Multiply)
                {
                    return (initialColor*newColor);
                }
            }
            return newColor;
        }

        protected override void ModifyVertices(List<UIVertex> verts)
        {

            if (this.gradientMode == GradientMode.Global)
            {
                UIVertex vertex3 = verts[0];
                Vector2 position = vertex3.position;
                UIVertex vertex4 = verts[verts.Count - 1];
                Vector2 vector2 = vertex4.position;
                for (int i = 0; i < verts.Count; i++)
                {
                    UIVertex vertex5 = verts[i];
                    if (vertex5.position.x < position.x)
                    {
                        UIVertex vertex6 = verts[i];
                        position.x = vertex6.position.x;
                    }
                    UIVertex vertex7 = verts[i];
                    if (vertex7.position.y > position.y)
                    {
                        UIVertex vertex8 = verts[i];
                        position.y = vertex8.position.y;
                    }
                    UIVertex vertex9 = verts[i];
                    if (vertex9.position.x > vector2.x)
                    {
                        UIVertex vertex10 = verts[i];
                        vector2.x = vertex10.position.x;
                    }
                    UIVertex vertex11 = verts[i];
                    if (vertex11.position.y < vector2.y)
                    {
                        UIVertex vertex12 = verts[i];
                        vector2.y = vertex12.position.y;
                    }
                }
                float num2 = position.y - vector2.y;
                float num3 = vector2.x - position.x;
                for (int j = 0; j < verts.Count; j++)
                {
                    UIVertex vertex = verts[j];
                    if (this.gradientDirection == GradientDirection.Vertical)
                    {
                        Color newColor = Color.Lerp(this.firstColor, this.secondColor,
                            (position.y - vertex.position.y)/num2);
                        vertex.color = this.CalculateColor((Color) vertex.color, newColor, this.colorMode);
                    }
                    else
                    {
                        Color color2 = Color.Lerp(this.firstColor, this.secondColor,
                            (vertex.position.x - position.x)/num3);
                        vertex.color = this.CalculateColor((Color) vertex.color, color2, this.colorMode);
                    }
                    if (this.useGraphicAlpha)
                    {
                        UIVertex vertex13 = verts[j];
                        vertex.color.a = (byte) ((vertex.color.a*vertex13.color.a)/0xff);
                    }
                    verts[j] = vertex;
                }
            }
            else
            {
                for (int k = 0; k < verts.Count; k++)
                {
                    UIVertex vertex2 = verts[k];
                    if (this.gradientDirection == GradientDirection.Vertical)
                    {
                        if ((((k%6) == 0) || ((k%6) == 1)) || ((k%6) == 5))
                        {
                            Color firstColor = this.firstColor;
                            vertex2.color = this.CalculateColor((Color) vertex2.color, firstColor, this.colorMode);
                        }
                        else
                        {
                            Color secondColor = this.secondColor;
                            vertex2.color = this.CalculateColor((Color) vertex2.color, secondColor, this.colorMode);
                        }
                    }
                    else if ((((k%6) == 0) || ((k%6) == 4)) || ((k%6) == 5))
                    {
                        Color color5 = this.firstColor;
                        vertex2.color = this.CalculateColor((Color) vertex2.color, color5, this.colorMode);
                    }
                    else
                    {
                        Color color6 = this.secondColor;
                        vertex2.color = this.CalculateColor((Color) vertex2.color, color6, this.colorMode);
                    }
                    if (this.useGraphicAlpha)
                    {
                        UIVertex vertex14 = verts[k];
                        vertex2.color.a = (byte) ((vertex2.color.a*vertex14.color.a)/0xff);
                    }
                    verts[k] = vertex2;
                }
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

        public Color firstColor
        {
            get { return this.m_FirstColor; }
            set
            {
                this.m_FirstColor = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public GradientDirection gradientDirection
        {
            get { return this.m_GradientDirection; }
            set
            {
                this.m_GradientDirection = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public GradientMode gradientMode
        {
            get { return this.m_GradientMode; }
            set
            {
                this.m_GradientMode = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public Color secondColor
        {
            get { return this.m_SecondColor; }
            set
            {
                this.m_SecondColor = value;
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

        public enum ColorMode
        {
            Override,
            Additive,
            Multiply
        }

        public enum GradientDirection
        {
            Vertical,
            Horizontal
        }

        public enum GradientMode
        {
            Local,
            Global
        }
    }

}