using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class ReorderableListBase
    {
    }

    [Serializable]
    public class ReorderableList<T> : ReorderableListBase
    {
        [SerializeField]
        private List<T> _list = new List<T>();

        public IList<T> list
        {
            get { return _list; }
        }
    }


    [Serializable]
    public class GameObjectReorderableList : ReorderableList<GameObject>
    {
    }
    [Serializable]
    public class MaterialReorderableList : ReorderableList<Material>
    {
    }
    [Serializable]
    public class TextureReorderableList : ReorderableList<Texture>
    {
    }
    [Serializable]
    public class ShaderReorderableList : ReorderableList<Shader>
    {
    }

    [Serializable]
    public class AnimationClipReorderableList : ReorderableList<AnimationClip>
    {
    }

    [Serializable]
    public class StringReorderableList : ReorderableList<String>
    {
    }
    [Serializable]
    public class IntReorderableList : ReorderableList<int>
    {
    }

   
}