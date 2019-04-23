using System;
using System.Collections;
using UnityEngine;

namespace foundation
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            T component = self.GetComponent<T>();
            if (component == null)
            {
                component = self.AddComponent<T>();
                //component.hideFlags=HideFlags.HideInInspector;
            }
            return component;
        }

        public static bool addEventListener(this GameObject self,string type, Action<EventX> listener, int priority = 0)
        {
            MonoEventDispatcher dispatcher=self.GetOrAddComponent<MonoEventDispatcher>();
            return dispatcher.addEventListener(type, listener, priority);
        }

        public static bool hasEventListener(this GameObject self,string type)
        {
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                return false;
            }
            return dispatcher.hasEventListener(type);
        }

        public static bool removeEventListener(this GameObject self,string type, Action<EventX> listener)
        {
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                return false;
            }
            return dispatcher.removeEventListener(type, listener);
        }

        public static bool dispatchEvent(this GameObject self,EventX e)
        {
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                return false;
            }
            return dispatcher.dispatchEvent(e);
        }

        public static bool simpleDispatch(this GameObject self,string type, object data = null)
        {
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                return false;
            }
            return dispatcher.simpleDispatch(type, data);
        }

        public static Transform RecursivelyFind(this GameObject go,string name)
        {
            return go.transform.RecursivelyFind(name);
        }

        public static Coroutine StartCoroutine(this GameObject self,IEnumerator routine)
        {
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                dispatcher = self.AddComponent<MonoEventDispatcher>();
            }
            return dispatcher.StartCoroutine(routine);
        }

        public static void StopCoroutine(this GameObject self, Coroutine routine)
        {
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                dispatcher = self.AddComponent<MonoEventDispatcher>();
            }
            dispatcher.StopCoroutine(routine);
        }

        public static void AddTween(this GameObject self, RFTweenerTask task)
        {
            TweenTaskMotor motor = self.GetOrAddComponent<TweenTaskMotor>();
            motor.addTask(task);
        }




        public static Transform GetChildTransform(this GameObject self, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            string fixName = name.ToLower();
            Transform temp = null;

            Transform[] trans = self.GetComponentsInChildren<Transform>(true);
            for (int i = 0, len = trans.Length; i < len; i++)
            {
                Transform tranTemp = trans[i];

                if (tranTemp.name.ToLower() == fixName)
                {
                    temp = tranTemp;
                    break;
                }
            }
            return temp;
        }

        public static Vector2 GetUIPosition(this GameObject self)
        {
            RectTransform rectTransform = self.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform.anchoredPosition;
            }
            else
            {
                return self.transform.position;
            }
        }

        public static void SetUIPosition(this GameObject self, Vector2 position)
        {
            RectTransform rectTransform=self.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = position;
            }
            else
            {
                self.transform.position = position;
            }
        }

        public static void SetUISize(this GameObject self, int width = -1, int height = -1)
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
        }

        public static Vector2 GetUISize(this GameObject go)
        {
            RectTransform rectTransform = go.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                return rectTransform.sizeDelta;
            }

            return go.transform.localScale;
        }
        public static void SetLayerRecursively(this GameObject self, string layerName)
        {
            LayerMask layer = LayerMask.NameToLayer(layerName);
            self.SetLayerRecursively(layer);
        }
        public static void SetLayerRecursively(this GameObject self, LayerMask layer)
        {
            self.layer = layer;
            Transform transform = self.transform;
            int len = transform.childCount;
            for (int i = 0; i < len; i++)
            {
                Transform childTransform = transform.GetChild(i);
                if (layer != childTransform.gameObject.layer)
                {
                    childTransform.gameObject.SetLayerRecursively(layer);
                }
            }
        }

        public static void SetRendererEnabledRecursive(this GameObject go, bool enabled)
        {
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = enabled;
            }
        }

        public static object GetData(this GameObject self)
        {
            MonoEventDispatcher dispatcher = self.GetComponent<MonoEventDispatcher>();
            if (dispatcher == null)
            {
                return null;
            }
            return dispatcher.data;
        }

        public static void SetData(this GameObject self,object value)
        {
            MonoEventDispatcher dispatcher = self.GetOrAddComponent<MonoEventDispatcher>();
            dispatcher.data=value;
        }
    }
}