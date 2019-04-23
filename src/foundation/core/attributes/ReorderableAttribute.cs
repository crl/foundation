using System;
using UnityEngine;

namespace foundation
{
    public class ReorderableAttribute : PropertyAttribute
    {
        public string title;
        public ReorderableAttribute()
        {
        }
        public ReorderableAttribute(string title="")
        {
            this.title = title;
        }
    }
}