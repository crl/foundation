using UnityEngine;

namespace foundation
{
    [AddComponentMenu("Lingyu/SpriteSheetUV")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(Renderer))]
    public class SpriteSheetUV : MonoBehaviour, ICanbeReplacBehaviour
    {
        [Range(1, 100)] public int fps = 1;
        [Range(1, 100)] public int countX = 2;
        [Range(1, 100)] public int countY = 1;

        [Range(0, 100)] public int startIndex = 0;
        [Range(0, 100)] public int count = 0;

        private int endIndex = 0;
        private int total = 1;

        private new Renderer renderer;
        private Material material;
        private float timef = 0;
        private Vector2 v;
        private Vector2 scaleTexture;

        public bool isTiling = false;
        private bool isVisibled = false;

        private bool hasMainTex = false;

        protected virtual void Awake()
        {
            AbstractApp.ReplacBehaviour(this);
        }
        protected virtual void OnEnable()
        {
            renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                if (Application.isPlaying)
                {
                    material = renderer.material;
                }
                else
                {
                    material = renderer.sharedMaterial;
                }
            }

            hasMainTex = material.HasProperty("_MainTex");
            if (hasMainTex == false)
            {
                return;
            }


            if (material != null && isTiling == false)
            {
                scaleTexture.x = 1.0f / countX;
                scaleTexture.y = 1.0f / countY;
                material.mainTextureScale = scaleTexture;
            }

            isVisibled = true;
            total = countX * countY;

            if (startIndex < 0)
            {
                startIndex = 0;
            }
            else if (startIndex >= total)
            {
                startIndex %= total;
            }

            if (count > 0)
            {
                endIndex = startIndex + count;
            }
            else
            {
                endIndex = total;
            }

            total = endIndex - startIndex;
            _lastFrame = -1;
        }


        public virtual void OnBecameVisible()
        {
            isVisibled = true;
        }

        public virtual void OnBecameInvisible()
        {
            isVisibled = false;
            timef = 0;
        }

        protected float _currentFrame;
        private float _lastFrame = -1;

        protected void Update()
        {
            if (isVisibled == false || hasMainTex==false)
            {
                return;
            }

            timef += TickManager.alwayDeltaTime;

            _currentFrame = startIndex + Mathf.Floor((timef * fps) % total);

            if (_currentFrame == _lastFrame)
            {
                return;
            }

            float flow = _currentFrame - endIndex;
            if (flow >= 0)
            {
                _currentFrame = startIndex + flow;
            }

            v.x = (_currentFrame % countX) / countX;
            v.y = -((int) (_currentFrame / countX)) / (float) countY;
            if (material)
            {
                material.mainTextureOffset = v;
            }

            _lastFrame = _currentFrame;
        }
    }
}