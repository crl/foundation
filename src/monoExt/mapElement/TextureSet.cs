using System;
using UnityEngine;

namespace foundation
{
    [Serializable]
    public class TextureSet
    {
        [Range(0,10)]
        public int index = 0;
        public Texture texture;

        public string name;
    }
}