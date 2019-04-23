namespace gameSDK
{
    using UnityEngine;

    public class UVAnimation : MonoBehaviour
    {
        /// <summary>
        /// 横向速度
        /// </summary>
        public float USpeed = 0;
        /// <summary>
        /// 纵向速度
        /// </summary>
        public float VSpeed = 0;
        /// <summary>
        /// 旋转速度(角度)
        /// </summary>
        public float WSpeed = 0;

        private MeshFilter meshFilter = null;
        private Mesh mesh = null;
        /// <summary>
        /// uv中心点
        /// </summary>
        public Vector2 centerUV = new Vector2(0.5F, 0.5F);
        private Vector2[] uvs = null;

        private Vector2 m_offset = new Vector2();
        private Renderer render;
        private bool hasMainText = false;
        public void Start()
        {
            if (WSpeed != 0)
            {
                meshFilter = GetComponent<MeshFilter>();
                mesh = meshFilter.mesh;
                uvs = mesh.uv;
            }

            render = transform.GetComponent<Renderer>();

            hasMainText = render.material.HasProperty("_MainTex");
        }

        void Update()
        {
            if (USpeed != 0 || VSpeed != 0)
            {
                m_offset.x += USpeed * Time.deltaTime;
                if (m_offset.x > 1)
                {
                    m_offset.x -= 1;
                }
                else if (m_offset.x < -1)
                {
                    m_offset.x += 1;
                }
                m_offset.y += VSpeed * Time.deltaTime;
                if (m_offset.y > 1)
                {
                    m_offset.y -= 1;
                }
                else if (m_offset.y < -1)
                {
                    m_offset.y += 1;
                }


                if (hasMainText)
                {
                    render.material.mainTextureOffset = m_offset;
                }
                else
                {
                    hasMainText = render.material.HasProperty("_MainTex");
                }
            }

            if (WSpeed != 0)
            {
                float angle = WSpeed * Mathf.Deg2Rad * Time.deltaTime;
                float angleSin = Mathf.Sin(angle);
                float angleCos = Mathf.Cos(angle);
                for (int i = 0; i < uvs.Length; i++)
                {
                    Vector2 diff = uvs[i] - centerUV;
                    uvs[i].x = diff.x * angleCos - diff.y * angleSin;
                    uvs[i].y = diff.x * angleSin + diff.y * angleCos;
                    uvs[i] += centerUV;
                }
                meshFilter.mesh.uv = uvs;

            }

        }
    }

}