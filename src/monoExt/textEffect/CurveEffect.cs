using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [RequireComponent(typeof (Text)), AddComponentMenu("UI/Effects/Curve Effect", 6)]
    public class CurveEffect : BaseVertexEffect
    {
        [SerializeField] private bool m_CenterOnCurveHalfPoint;
        [SerializeField] private bool m_CharacterCountScalesStrength;
        [SerializeField] private AnimationCurve m_Curve;
        [SerializeField] private float m_Strength;

        protected CurveEffect()
        {
            Keyframe[] keys = new Keyframe[] {new Keyframe(0f, 0f, 0f, 2f), new Keyframe(1f, 0f, -2f, 0f)};
            this.m_Curve = new AnimationCurve(keys);
            this.m_Strength = 1f;
            this.m_CharacterCountScalesStrength = true;
            this.m_CenterOnCurveHalfPoint = true;
        }

        protected override void ModifyVertices(List<UIVertex> verts)
        {
            if ((verts.Count != 0))
            {
                UIVertex vertex2 = verts[0];
                Vector2 position = vertex2.position;
                UIVertex vertex3 = verts[verts.Count - 1];
                Vector2 vector2 = vertex3.position;
                float strength = this.strength;
                if (this.m_CharacterCountScalesStrength)
                {
                    int num2 = (verts.Count - 4)/4;
                    strength = this.strength*num2;
                }
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
                float num4 = vector2.x - position.x;
                for (int j = 0; j < verts.Count; j++)
                {
                    UIVertex vertex = verts[j];
                    vertex.position.y += this.curve.Evaluate((vertex.position.x - position.x)/num4)*strength;
                    if (this.m_CenterOnCurveHalfPoint)
                    {
                        vertex.position.y += this.curve.Evaluate(0.5f)*-strength;
                    }
                    verts[j] = vertex;
                }
            }
        }

        public AnimationCurve curve
        {
            get { return this.m_Curve; }
            set
            {
                if (this.m_Curve != value)
                {
                    this.m_Curve = value;
                    if (base.graphic != null)
                    {
                        base.graphic.SetVerticesDirty();
                    }
                }
            }
        }

        public float strength
        {
            get { return this.m_Strength; }
            set
            {
                if (this.m_Strength != value)
                {
                    this.m_Strength = value;
                    if (base.graphic != null)
                    {
                        base.graphic.SetVerticesDirty();
                    }
                }
            }
        }
    }

}