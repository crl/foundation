using UnityEngine;

namespace gameSDK
{
    [AddComponentMenu("Lingyu/HTLiquidSpriteSheet")]
    public class HTLiquidSpriteSheet : MonoBehaviour
    {
        public int uvAnimationTileX = 8;
        public int uvAnimationTileY = 8;

        [Range(1, 100)]
        public int framesPerSecond = 26;
        public Vector2 scrollSpeed;

        private float _startTime;
        private Vector2[] offsets;
        private int spriteCount = 64;
        private Vector2 currentOffset;
        private Material material;

        private void Start()
        {
            spriteCount = uvAnimationTileX * uvAnimationTileY;

            offsets = new Vector2[spriteCount];
            _startTime = Time.time;

            material = GetComponent<Renderer>().material;

            float xSpriteSize = 1.0f / uvAnimationTileX;
            float ySpriteSize = 1.0f / uvAnimationTileY;

            Vector2 textureSize = new Vector2(xSpriteSize, ySpriteSize);

            material.SetTextureScale("_MainTex", textureSize);

            int i = 0;
            for (int y = 0; y < uvAnimationTileY; y++)
            {
                for (int x = 0; x < uvAnimationTileX; x++)
                {
                    offsets[i] = new Vector2(x * xSpriteSize, 1.0f - y * ySpriteSize);
                    i++;
                }
            }
        }

        private int preIndex = -1;
        private void Update()
        {
            int index = (int)((Time.time - _startTime) * framesPerSecond);
            if (preIndex == index)
            {
                return;
            }
            preIndex = index;
            index = index % spriteCount;

            if (index == spriteCount)
            {
                _startTime = Time.time;
                index = 0;
            }

            currentOffset += scrollSpeed * Time.deltaTime;
            material.SetTextureOffset("_MainTex", offsets[(int)index] + currentOffset);
        }
    }
}