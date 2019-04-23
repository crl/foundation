using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Lingyu/TextureRef")]
    public class TextureRef : MonoBehaviour
    {
        public List<TextureSet> textureSet;
        /// <summary>
        /// 查找不到时的,错误显示图
        /// </summary>
        public Texture errorTexture;
        public Texture getTexture(string name)
        {
            foreach (TextureSet tSet in textureSet)
            {
                Texture item = tSet.texture;
                if (item == null)
                {
                    continue;
                }

                if (tSet.name == name || item.name == name)
                {
                    return item;
                }
            }

            return errorTexture;
        }
    }
}
