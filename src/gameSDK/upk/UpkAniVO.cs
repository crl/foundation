using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    [Serializable]
    public class UpkAniVO : ScriptableObject
    {
        // Fields
        public int fps = 10;
        [SerializeField]
        public List<SpriteInfoVO> keys;
        public bool loop = false;
        // Methods
        public Sprite GetSpriteByName(string name)
        {
            if (keys == null)
            {
                Debug.Log("UpkAniVO keys is null");
                return null;
            }
            for (int i = 0; i < this.keys.Count; i++)
            {
                SpriteInfoVO item = this.keys[i];

                if (item == null)
                {
                    Debug.Log("UpkAniVO error is null at:" + i);
                    continue;
                }

                if (item.name == name)
                {
                    return this.keys[i].sprite;
                }
            }
            return null;
        }
    }
}