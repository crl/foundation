using System;
using UnityEngine;

namespace foundation
{
    public static class MonoBehaviourExtension
    {
        public static bool addEventListener(this Component self, string type, Action<EventX> listener, int priority = 0)
        {
            MonoEventDispatcher dispatcher = self.gameObject.GetOrAddComponent<MonoEventDispatcher>();
            return dispatcher.addEventListener(type, listener, priority);
        }

        /// <summary>
        /// 判断世界坐标系内的物体,摄像机是否可见
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool isVisibleOnCamera(this Component self, Camera camera)
        {
            return self.transform.isVisibleOnCamera(camera);
        }
        public static bool isVisibleOnCamera(this GameObject self, Camera camera)
        {
            return self.transform.isVisibleOnCamera(camera);
        }
        public static bool isVisibleOnCamera(this Transform self, Camera camera)
        {
            if (camera == null)
            {
                return false;
            }
            Vector3 pos = camera.WorldToViewportPoint(self.position);
            // Determine the visibility and the target alpha
            bool isVisible = (camera.orthographic || pos.z > 0f) && (pos.x > 0f && pos.x < 1f && pos.y > 0f && pos.y < 1f);
            return isVisible;
        }

        public static bool hasEventListener(this Component self, string type)
        {                   
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                return false;
            }
            return dispatcher.hasEventListener(type);
        }

        public static bool removeEventListener(this Component self, string type, Action<EventX> listener)
        {
#if UNITY_EDITOR
            ///编辑器直接退出时会有这种贱的调用
            if(self==null)return false;
#endif
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                return false;
            }
            return dispatcher.removeEventListener(type, listener);
        }

        public static bool dispatchEvent(this Component self, EventX e)
        {
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                return false;
            }
            return dispatcher.dispatchEvent(e);
        }

        public static bool simpleDispatch(this Component self, string type, object data = null)
        {
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                return false;
            }
            return dispatcher.simpleDispatch(type, data);
        }

        public static void SetActive(this Component self, bool value)
        {
            if (self != null && self.gameObject != null && self.gameObject.activeSelf != value)
            {
                self.gameObject.SetActive(value);
            }
        }

        public static void SetUIPosition(this Component self, Vector2 position)
        {
            RectTransform rectTransform = self.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = position;
            }
            else
            {
                self.transform.localPosition = position;
            }
        }

        public static void SetUIPivot(this Component self, Vector2 position)
        {
            RectTransform rectTransform = self.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.pivot = position;
            }
        }

        public static Vector2 GetUIPosition(this Component self)
        {
            RectTransform rectTransform = self.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform.anchoredPosition;
            }
            else
            {
                return self.transform.localPosition;
            }
        }

        public static void SetUISize(this Component self, Vector2 sizeDelta)
        {
            RectTransform rectTransform = self.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = sizeDelta;
            }
        }
        public static void SetUISize(this Component self, int width = -1, int height = -1)
        {
            RectTransform rectTransform = self.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 sizeDelta = rectTransform.sizeDelta;
                if (width != -1)
                {
                    sizeDelta.x = width;
                }
                if (height != -1)
                {
                    sizeDelta.y = height;
                }
                rectTransform.sizeDelta = sizeDelta;
            }
            else
            {
               //todo;
            }
        }

        public static Vector2 GetUISize(this Component self)
        {
            RectTransform rectTransform = self.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
               return rectTransform.sizeDelta;
            }

            return self.transform.localScale;
        }
    }
}