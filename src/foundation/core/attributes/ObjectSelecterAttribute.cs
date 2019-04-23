using System;
using UnityEngine;

namespace foundation
{
    public class ObjectSelecterAttribute : PropertyAttribute
    {
        public Type type;
        public string path;
        public ObjectSelecterAttribute(Type type,string path="Assets")
        {
            this.path = path;
            this.type = type;
        }
    }
}