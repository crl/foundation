using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace foundation
{
    [AddComponentMenu("Lingyu/BoundariesCFG")]
    public class BoundariesCFG : MonoCFG
    {
        public BoundCFG[] list = new BoundCFG[0];
#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            foreach (BoundCFG cfg in list)
            {
                cfg.OnDrawGizmos();
            }
        }
#endif
        protected void Start()
        {
            GenerateColliders();
        }

        private bool isForceUpdate = false;
        private void FixedUpdate()
        {
            if (Application.isEditor && isForceUpdate)
            {
                GenerateColliders();
            }
        }

        private void GenerateColliders()
        {
            BoxCollider[] children = GetComponentsInChildren<BoxCollider>();
            for (int i = children.Length-1; i >-1; i--)
            {
                GameObject.Destroy(children[i].gameObject);
            }

            int index = 0;
            foreach (BoundCFG cfg in list)
            {
                cfg.GenerateColliders(gameObject,"group_"+index);
                index++;
            }
        }
    }

    [System.Serializable]
    public class BoundCFG
    {
        public List<Segment> segments = new List<Segment>();
        public DepthAnchorTypes depthAnchor= DepthAnchorTypes.Middle;

        [Range(0.1f,10f)]
        public float height=2.0f;
        [Range(0.1f, 10f)]
        public float depth=0f;
        public Color color=new Color(0,1,0,0.5f);

        public bool isClosed;
        public LayerMask layer;

        public Vector3 Centroid
        {
            get
            {
                if (segments.Count > 0)
                {
                    var sum = Vector3.zero;

                    for (int i = 0; i < segments.Count; i++)
                    {
                        sum += segments[i].Midpoint;
                    }

                    return sum / segments.Count;
                }
                else {
                    return Vector3.zero;
                }
            }
        }

        public void Connect()
        {
            for (int i = 0; i < segments.Count - 1; i++)
            {
                segments[i].end = segments[i + 1].start;
            }

            if (isClosed && segments.Count > 2)
            {
                segments[segments.Count - 1].end = segments[0].start;
            }
        }

        public void Translate(Vector3 delta)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                segments[i].start += delta;
                segments[i].end += delta;
            }
        }

#if UNITY_EDITOR
        internal void OnDrawGizmos()
        {
            Mesh mesh;
            List<Mesh> segmentMeshes = new List<Mesh>();

            Material material = new Material(Shader.Find("Hidden/Internal-Colored"));
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Cull", (int) CullMode.Back);

            material.hideFlags = HideFlags.HideAndDontSave;
            material.color = color;

            for (int i = 0; i < segments.Count; i++)
            {
                segmentMeshes.Add(
                    segments[i].GetMesh(height, depth, depthAnchor)
                );
            }
            if (segments.Count == 1)
            {
                mesh = segmentMeshes[0];
            }
            else {
                CombineInstance[] combine = new CombineInstance[segmentMeshes.Count];
                for (int i = 0; i < segmentMeshes.Count; i++)
                {
                    combine[i].mesh = segmentMeshes[i];
                }
                mesh = new Mesh()
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                mesh.CombineMeshes(combine, true, false);
                segmentMeshes.ForEach(sm => GameObject.DestroyImmediate(sm));
            }

            for (int i = 0; i < material.passCount; i++)
            {
                if (material.SetPass(i))
                {
                    Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
                }
            }

            GameObject.DestroyImmediate(material);
            GameObject.DestroyImmediate(mesh);
        }
#endif



        internal void GenerateColliders(GameObject parent,string prefix)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                Segment segment = segments[i];
                GameObject seg = new GameObject(prefix+"_segments_" + i);
                seg.transform.parent = parent.transform;
                seg.layer = layer;

                var bc = seg.AddComponent<BoxCollider>();
                bc.size = segment.GetSize(height, depth);
                bc.center = segment.GetCenter(height, depth, depthAnchor);

                Quaternion rot = segment.GetYAxisRotation();
                if (rot != Quaternion.identity)
                {
                    seg.transform.rotation = rot;
                }
                seg.transform.Rotate(0f, 90f, 0f);
                seg.transform.position = segment.Midpoint;
            }
        }
    }
}

    public enum DepthAnchorTypes
    {
        Middle,
        Left,
        Right
    }

    [System.Serializable]
    public class Segment
    {
        public Vector3 start;
        public Vector3 end;

        public Vector3 Midpoint
        {
            get
            {
                return Vector3.Lerp(start, end, 0.5f);
            }
        }

        public Vector3 GetSize(float height, float depth)
        {
            return new Vector3(
                Vector3.Distance(start, end),
                height,
                depth
            );
        }

        public Vector3 GetCenter(float height, float depth, DepthAnchorTypes depthAnchor)
        {
            var center = new Vector3();

            switch (depthAnchor)
            {
                case DepthAnchorTypes.Middle:
                    center = new Vector3(0.0f, height / 2.0f, 0.0f);
                    break;

                case DepthAnchorTypes.Left:
                    center = new Vector3(0.0f, height / 2.0f, depth / 2.0f);
                    break;

                case DepthAnchorTypes.Right:
                    center = new Vector3(0.0f, height / 2.0f, depth / 2.0f * -1);
                    break;
            }

            return center;
        }

        /// <summary>
        /// Gets the Y axis rotation.
        /// </summary>
        /// <returns>The Y axis rotation</returns>
        public Quaternion GetYAxisRotation()
        {
            var dir = (start - end).normalized;

            return (dir == Vector3.zero) ? Quaternion.identity : Quaternion.LookRotation(dir);
        }

#if UNITY_EDITOR
        public Mesh GetMesh(float height, float depth, DepthAnchorTypes depthAnchor)
        {
            Vector3 s1 = new Vector3();
            Vector3 s2 = new Vector3();
            Vector3 e1 = new Vector3();
            Vector3 e2 = new Vector3();

            var p1 = new Vector3();
            var p2 = new Vector3();
            var p3 = new Vector3();
            var p4 = new Vector3();
            var p5 = new Vector3();
            var p6 = new Vector3();
            var p7 = new Vector3();
            var p8 = new Vector3();

            if (depth == 0.0f)
            {
                p1 = start;
                p2 = start + Vector3.up * height;
                p3 = end + Vector3.up * height;
                p4 = end;

                p5 = start;
                p6 = start + Vector3.up * height;
                p7 = end + Vector3.up * height;
                p8 = end;
            }
            else {
                switch (depthAnchor)
                {
                    case DepthAnchorTypes.Middle:
                        s1 = start;
                        s2 = start;

                        s1.x -= depth / 2.0f;
                        s2.x += depth / 2.0f;

                        e1 = end;
                        e2 = end;

                        e1.x -= depth / 2.0f;
                        e2.x += depth / 2.0f;

                        break;

                    case DepthAnchorTypes.Left:

                        s1 = start;
                        s2 = start;

                        s1.x += depth;

                        e1 = end;
                        e2 = end;

                        e1.x += depth;

                        break;

                    case DepthAnchorTypes.Right:

                        s1 = start;
                        s2 = start;

                        s1.x -= depth;

                        e1 = end;
                        e2 = end;

                        e1.x -= depth;

                        break;
                }

                // rotate wireframe box
                s1 = RotateAroundPoint(s1, start, GetYAxisRotation());
                s2 = RotateAroundPoint(s2, start, GetYAxisRotation());
                e1 = RotateAroundPoint(e1, end, GetYAxisRotation());
                e2 = RotateAroundPoint(e2, end, GetYAxisRotation());

                p1 = s1;
                p2 = s1 + Vector3.up * height;
                p3 = e1 + Vector3.up * height;
                p4 = e1;

                p5 = s2;
                p6 = s2 + Vector3.up * height;
                p7 = e2 + Vector3.up * height;
                p8 = e2;
            }

            Mesh mesh = new Mesh
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            mesh.vertices = new[] { p1, p2, p3, p4, p5, p6, p7, p8 };

            mesh.triangles = new[]{
                0,2,1,1,2,0,
                0,2,3,3,2,0,
                4,6,5,5,6,4,
                4,6,7,7,6,4,
                0,5,1,1,5,0,
                0,5,4,4,5,0,
                1,6,5,5,6,1,
                1,6,2,2,6,1,
                0,7,4,4,7,0,
                0,7,3,3,7,0,
                2,7,6,6,7,2,
                2,7,3,3,7,2
            };

            Vector3[] vertices = mesh.vertices;
            Vector2[] uvs = new Vector2[vertices.Length];
            int j = 0;
            while (j < uvs.Length)
            {
                uvs[j] = new Vector2(vertices[j].x, vertices[j].z);
                j++;
            }

            mesh.uv = uvs;
            mesh.RecalculateNormals();

            return mesh;
        }
#endif

        /// <summary>
        /// Rotates the around point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="pivot">The pivot.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns></returns>
        private static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }
    }