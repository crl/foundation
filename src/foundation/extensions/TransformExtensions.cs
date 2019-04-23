using UnityEngine;

namespace foundation
{
    public static class TransformExtensions
    {
        public static void CopyFrom(this Transform self, Transform other, bool localSpace)
        {
            if (localSpace)
            {
                self.localPosition = other.localPosition;
                self.localRotation = other.localRotation;
                self.localScale = other.localScale;
            }
            else
            {
                self.position = other.position;
                self.rotation = other.rotation;
                self.SetWorldScale(other.lossyScale);
            }
        }

        public static Transform RecursivelyFind(this Transform self, string name)
        {
            int len = self.childCount;
            for (int i = 0; i < len; i++)
            {
                Transform transform = self.GetChild(i);
                if (transform.name == name)
                {
                    return transform;
                }
                transform = transform.RecursivelyFind(name);
                if (transform != null)
                {
                    return transform;
                }
            }
            return null;
        }

        public static void DestroyChildren(this Transform self)
        {
            Transform[] componentsInChildren = self.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i] != self)
                {
                    UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
                }
            }
        }

        public static void DestroyChildrenImmediate(this Transform self)
        {
            Transform[] componentsInChildren = self.GetComponentsInChildren<Transform>(true);
            for (int i = componentsInChildren.Length - 1; i >= 0; i--)
            {
                if (componentsInChildren[i] != self)
                {
                    UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
                }
            }
        }


        public static bool IsSiblingOf(this Transform self, Transform other)
        {
            return (self.parent == other.parent);
        }

        public static void ScaleBy(this Transform self, float scaleBy)
        {
            self.localScale = (self.localScale * scaleBy);
        }

        public static void ScaleBy(this Transform self, Vector3 scaleBy)
        {
            Vector3 localScale = self.localScale;
            localScale.Scale(scaleBy);
            self.localScale = localScale;
        }

        public static void SetParentAndStayLocal(this Transform self, Transform parent)
        {
            Vector3 localPosition = self.localPosition;
            Quaternion localRotation = self.localRotation;
            Vector3 localScale = self.localScale;
            self.SetParent(parent);
            self.localPosition = localPosition;
            self.localRotation = localRotation;
            self.localScale = localScale;
        }

        public static void SetWorldScale(this Transform self, Vector3 scale)
        {
            self.localScale = Vector3.one;
            Vector3 lossyScale = self.lossyScale;
            if (lossyScale.x != 0f)
            {
                scale.x /= lossyScale.x;
            }
            if (lossyScale.y != 0f)
            {
                scale.y /= lossyScale.y;
            }
            if (lossyScale.z != 0f)
            {
                scale.z /= lossyScale.z;
            }
            self.localScale = scale;
        }
    }
}