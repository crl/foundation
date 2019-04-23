using UnityEngine;

namespace foundation
{

    public enum PathSelecterType
    {
        NPC,
        MONSTER,
        MAP,

        SKILL,
        STOTY,

        SKILL_STOTY,
    }
    public class PathSelecterAttribute:PropertyAttribute
    {
        public string extention;
        public PathSelecterType type;
        public PathSelecterAttribute(PathSelecterType type,string extention="amf")
        {
            this.type = type;
            this.extention = extention;
        }
    }
}