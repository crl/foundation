using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    public class UILocater
    {
        public static GameObject CanvasLayer { get; internal set; }
        public static GameObject PopUpLayer { get; internal set; }
        public static GameObject TipLayer { get; internal set; }
        public static GameObject UILayer { get; internal set; }
        public static GameObject FollowLayer { get; internal set; }


        /// <summary>
        /// 临时的UILayer;
        /// </summary>
        //public static GameObject TempUILayer;
        public static CanvasScaler CanvasScaler { get; internal set; }

        public static bool layerFullScreen = false;
        public static void initialize(Canvas canvas, CanvasScaler canvasScaler)
        {
            UILocater.CanvasScaler = canvasScaler;
            UILocater.CanvasLayer = canvas.gameObject;

            UILocater.FollowLayer = CreateEmptyLayer("FollowLayer", CanvasLayer);
            UILocater.UILayer = CreateEmptyLayer("UILayer", CanvasLayer);
            UILocater.TipLayer = CreateEmptyLayer("TipLayer", CanvasLayer);
            UILocater.PopUpLayer = CreateEmptyLayer("PopUpLayer", CanvasLayer);
        }

        public static void ResetAllFullScreen(bool value)
        {
            layerFullScreen = value;
            ResetFullScreen(FollowLayer,value);
            ResetFullScreen(UILayer, value);
            ResetFullScreen(TipLayer, value);
            ResetFullScreen(PopUpLayer, value);
        }

        public static void ResetFullScreen(GameObject layer, bool value)
        {
            RectTransform rect = layer.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            if (value)
            {
                rect.anchorMax = Vector2.one;
                rect.anchorMin = Vector2.zero;
            }
            else
            {
                Vector2 v = new Vector2(0.5f, 0.5f);
                rect.anchorMax = v;
                rect.anchorMin = v;
            }
        }

        public static GameObject CreateEmptyLayer(string name = "Image", GameObject parent = null)
        {
            GameObject go = new GameObject(name);
            RectTransform rect=go.AddComponent<RectTransform>();
            go.layer = LayerMask.NameToLayer("UI");
            if (parent == null)
            {
                parent = UILocater.CanvasLayer;
            }

            go.transform.SetParent(parent.transform, false);

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);

            if (layerFullScreen)
            {
                rect.anchorMax = Vector2.one;
                rect.anchorMin = Vector2.zero;
            }
            else {
                Vector2 v = new Vector2(0.5f, 0.5f);
                rect.anchorMax = v;
                rect.anchorMin = v;
            }
            return go;
        }


        public static void ToggleLayer(GameObject layer, bool isShow=true,bool useActive=false)
        {
            if (layer == null)
            {
                return;
            }

            ///直接关闭的形式(会触发onAdd/onRemve StageHandle)
            if (useActive)
            {
                if (layer.activeSelf != isShow)
                {
                    layer.SetActive(isShow);
                }
                return;
            }

            ///使用透明来做隐藏(不触发事件)
            CanvasGroup canvasGroup = layer.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = layer.AddComponent<CanvasGroup>();
            }

            if (isShow)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1;
            }
            else
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0;
            }
        }
    }
}