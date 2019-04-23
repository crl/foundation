using UnityEngine;

namespace gameSDK
{
    public class SequenceFrameWithUVAnim : MonoBehaviour
    {
        public float m_fDelay = 0.0f;
        private bool m_bDelayEnd = false;

        private float m_timeElasped = 0.0f;
        protected int m_curFrame = 0;
        private int m_lastFrame = 0;
        public float m_fps = 10.0f;
        protected float m_oneFrameTime;

        //public int m_row = 1;
        //public int m_column = 1;
        public int m_totalFrames = 1;

        public int m_loop = 0;
        protected int m_curLoop = 0;

        protected bool m_bStop = false;

        public bool m_isVMove = false;
        public float m_animSpeed = 0.0f;

        private float[] m_fDelta = null;
        private Material[] materials;

        private void Start()
        {
            materials = GetComponent<Renderer>().materials;
            m_fDelta = new float[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                m_fDelta[i] = (m_isVMove ? materials[i].mainTextureScale.x : materials[i].mainTextureScale.y)/
                              (float) m_totalFrames;

                materials[i].mainTextureOffset = new Vector2(0, m_isVMove ? 0 : (1 - m_fDelta[i]));
                materials[i].mainTextureScale = m_isVMove
                    ? new Vector2(m_fDelta[i], materials[i].mainTextureScale.y)
                    : new Vector2(materials[i].mainTextureScale.x, m_fDelta[i]);
            }

            m_oneFrameTime = 1.0f/m_fps;
        }

        private void Update()
        {
            if (!m_bDelayEnd)
            {
                m_timeElasped += Time.deltaTime;
                if (m_timeElasped >= m_fDelay)
                {
                    m_bDelayEnd = true;
                    m_timeElasped = 0.0f;
                }
                else
                {
                    return;
                }
            }

            if (m_bStop)
            {
                return;
            }

            m_timeElasped += Time.deltaTime;
            if (m_timeElasped >= m_oneFrameTime)
            {
                m_timeElasped = 0;
                m_curFrame++;
            }

            if (m_curFrame >= m_totalFrames)
            {
                m_curFrame = 0;

                if (m_loop != 0)
                {
                    m_curLoop++;
                    if (m_curLoop >= m_loop)
                    {
                        m_bStop = true;
                        return;
                    }
                }
            }

            if (m_curFrame != m_lastFrame)
            {
                nextFrame();
            }

            m_lastFrame = m_curFrame;

            uvMove();
        }

        private void nextFrame()
        {
            int nFrameNum = m_curFrame%m_totalFrames;

            //Debug.LogWarning("Next Frame");
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].mainTextureOffset = m_isVMove
                    ? new Vector2(nFrameNum*m_fDelta[i], materials[i].mainTextureOffset.y)
                    : new Vector2(materials[i].mainTextureOffset.x, (m_totalFrames - nFrameNum - 1)*m_fDelta[i]);
            }
        }

        protected void uvMove()
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].mainTextureOffset += m_isVMove
                    ? new Vector2(0, -m_animSpeed*Time.deltaTime)
                    : new Vector2(m_animSpeed*Time.deltaTime, 0);
            }
        }
    }
}
