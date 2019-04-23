using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Lingyu/SpriteRef")]
    public class SpriteRef : MonoBehaviour,ICanbeReplacBehaviour
    {
        /// <summary>
        /// 所有图像列表
        /// </summary>
        public List<SpriteSet> spriteSet;

        protected virtual void Awake()
        {
            AbstractApp.ReplacBehaviour(this);
        }

        /// <summary>
        /// 查找不到时的,错误显示图
        /// </summary>
        public Sprite errorSprite;
        public Sprite getSprite(string name)
        {
            foreach (SpriteSet sSet in spriteSet)
            {
                Sprite item = sSet.sprite;
                if (item == null)
                {
                    continue;
                }
                if (sSet.name == name || item.name==name)
                {
                    return item;
                }
            }

            return errorSprite;
        }
    }
}
