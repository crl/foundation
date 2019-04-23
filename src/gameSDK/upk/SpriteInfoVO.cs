using System;
using UnityEngine;

namespace foundation
{
    [Serializable]
    public class SpriteInfoVO
    {
        // Fields
        public float delay = 0f;
        public Sprite sprite;

        // Properties
        public string name
        {
            get
            {
                return this.sprite.name;
            }
        }
    }
}