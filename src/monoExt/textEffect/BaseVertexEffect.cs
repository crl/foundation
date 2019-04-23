using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    public class BaseVertexEffect:BaseMeshEffect
    {
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
                return;

            List<UIVertex> vertexList =ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(vertexList);

            ModifyVertices(vertexList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);

            ListPool<UIVertex>.Release(vertexList);
        }

        protected virtual void ModifyVertices(List<UIVertex> vertexList)
        {
        }
    }
}