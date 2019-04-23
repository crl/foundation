using UnityEngine;

namespace foundation
{
    public class UVAnimationV2 : MonoBehaviour
    {
        public bool m_bSingleMaterial = false;
        public int m_materialIndex = 0;

        public float m_fDelay = 0.0f;

        public float m_fTilingXStart = 0.0f;
        public float m_fTilingXEnd = 0.0f;
        public float m_fTilingXTime = 0.0f;

        public float m_fTilingYStart = 0.0f;
        public float m_fTilingYEnd = 0.0f;
        public float m_fTilingYTime = 0.0f;

        public float m_fUSpeed = 0.0f;
        public float m_fVSpeed = 0.0f;

        private bool m_bDelayEnd = false;
        private float m_fAccumTime = 0.0f;
        private Material[] materials;

        private void Start()
        {
            materials = GetComponent<Renderer>().materials;
        }

        private void Update()
        {
            if (!m_bDelayEnd)
            {
                m_fAccumTime += Time.deltaTime;
                if (m_fAccumTime >= m_fDelay)
                {
                    m_bDelayEnd = true;
                    m_fAccumTime = 0.0f;
                }
                else
                {
                    return;
                }
            }

            m_fAccumTime += Time.deltaTime;

            float x = Mathf.Lerp(m_fTilingXStart, m_fTilingXEnd, m_fAccumTime/m_fTilingXTime);
            float y = Mathf.Lerp(m_fTilingYStart, m_fTilingYEnd, m_fAccumTime/m_fTilingYTime);
            Material material;
            if (m_bSingleMaterial)
            {
                material = materials[m_materialIndex];
                Vector2 oldUV = material.mainTextureOffset;
                //materials[m_materialIndex].mainTextureOffset = new Vector2(oldUV.x + m_fUSpeed, oldUV.y + m_fVSpeed);
                float uVal = oldUV.x + m_fUSpeed*Time.deltaTime*60.0f;
                oldUV.x = uVal - (int) (uVal);
                float vVal = oldUV.y + m_fVSpeed*Time.deltaTime*60.0f;
                oldUV.y = vVal - (int) (vVal);
                materials[m_materialIndex].mainTextureOffset = oldUV;

                //materials[m_materialIndex].SetTextureScale("_MainTex", new Vector2(x, y));
                Vector2 oldScale = material.mainTextureScale;
                oldScale.x = x;
                oldScale.y = y;
                material.mainTextureScale = oldScale;
            }
            else
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    material = materials[i];
                    Vector2 oldUV = material.mainTextureOffset;
                    //materials[i].mainTextureOffset = new Vector2(oldUV.x + m_fUSpeed, oldUV.y + m_fVSpeed);
                    float uVal = oldUV.x + m_fUSpeed*Time.deltaTime*60.0f;
                    oldUV.x = uVal - (int) (uVal);
                    float vVal = oldUV.y + m_fVSpeed*Time.deltaTime*60.0f;
                    oldUV.y = vVal - (int) (vVal);
                    materials[i].mainTextureOffset = oldUV;

                    //materials[i].SetTextureScale("_MainTex", new Vector2(x, y));
                    Vector2 oldScale = material.mainTextureScale;
                    oldScale.x = x;
                    oldScale.y = y;
                    material.mainTextureScale = oldScale;
                }
            }
        }
    }
}
