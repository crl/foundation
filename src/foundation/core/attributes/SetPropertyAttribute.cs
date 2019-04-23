using UnityEngine;

namespace foundation
{
    public class SetPropertyAttribute : PropertyAttribute
    {
        public string Name { get; private set; }
        public bool IsDirty { get; set; }

        public SetPropertyAttribute(string name)
        {
            this.Name = name;
        }
    }
}