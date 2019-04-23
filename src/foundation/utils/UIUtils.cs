using System.Collections.Generic;
using clayui;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace foundation
{
    /// <summary>
    /// 
    /// </summary>
    public class UIUtils
    {
        public static string shareGrayShaderPath = "Lingyu/UI/GrayColor";

        private static Font _defalutFont;
        public static Font DefaultFont
        {
            get
            {
                if (_defalutFont == null)
                {
                    _defalutFont = Resources.Load<Font>("font");
                }

                return _defalutFont;
            }
            set { _defalutFont = value; }
        }

        public static Text CreateText(string name="Text", int fontSize = 30, GameObject parent=null)
        {
            GameObject go = CreateEmpty(name, parent);

            Text text = go.AddComponent<Text>();
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = UIUtils.DefaultFont;
            text.color = Color.white;

            return text;
        }

        public static void SetSize(GameObject go, int width = -1, int height = -1)
        {
            go.SetUISize(width, height);
        }

        public static void SetPosition(GameObject go, Vector2 position)
        {
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = position;
            }
            else
            {
                go.transform.localPosition = position;
            }
        }
        public static Vector2 GetPosition(GameObject go)
        {
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform.anchoredPosition;
            }
            return go.transform.localPosition;
        }

        public static Vector2 GetSize(GameObject go)
        {
            return go.GetUISize();
        }

        private static ASDictionary<Color,Texture> texture2Dictionary=new ASDictionary<Color, Texture>(); 
        public static Texture GetSharedColorTexture(Color color)
        {
            Texture result;
            if (texture2Dictionary.TryGetValue(color, out result) == false || result == null)
            {
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D.SetPixel(1, 1, color);
                texture2D.Apply();

                result = texture2D;
                texture2Dictionary[color] = result;
            }

            return result;
        }

        private static Sprite sprite;
        public static Sprite GetSharedCDSprite()
        {
            if (sprite != null)
            {
                return sprite;
            }
            Texture texture = GetSharedColorTexture(new Color(0, 0, 0, 0.5f));
            if (texture != null)
            {
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Vector2 p = new Vector2();
                sprite=Sprite.Create(texture as Texture2D, rect,p);
            }
            return sprite;
        }
        public static int GetNextPowerOfTwo(float value)
        {
            int result = 1;
            value -= 0.000000001f; // avoid floating point rounding errors

            while (result < value) result <<= 1;
            return result;
        }

        public static ScrollRect CreateScrollRect(string name= "ScrollRect", GameObject parent = null)
        {
            GameObject go = CreateEmpty(name, parent);
            ScrollRect scrollRect = go.AddComponent<ScrollRect>();
            return scrollRect;
        }

        public static GridLayoutGroup CreateGridLayout(string name = "Grid", GameObject parent = null)
        {
            GameObject go = CreateEmpty(name, parent);
            GridLayoutGroup grid = go.AddComponent<GridLayoutGroup>();
            return grid;
        }

        public static GameObject CreateEmpty(string name = "UIImage", GameObject parent = null)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            go.AddComponent<CanvasRenderer>();
            go.layer = LayerMask.NameToLayer("UI");
            if (parent == null)
            {
                parent = UILocater.CanvasLayer;
            }

            go.transform.SetParent(parent.transform, false);
            return go;
        }

        public static Canvas CreateCanvas(string name="Canvas",Camera camera=null ,GameObject parent = null)
        {
            GameObject go = CreateEmpty(name,parent);
            go.AddComponent<RectTransform>();
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.pixelPerfect = true;
            canvas.worldCamera = camera;
            canvas.planeDistance = 10;
            canvas.sortingLayerName = "UI";

            CanvasScaler canvasScaler = go.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
            canvasScaler.referencePixelsPerUnit = 100f;

            go.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        public static Image CreateImage(string name="Image", GameObject parent=null)
        {
            GameObject go = CreateEmpty(name, parent);
            Image image = go.AddComponent<Image>();
            image.raycastTarget = false;
            return image;
        }

        public static RawImage CreateRawImage(string name = "rawImage", GameObject parent = null)
        {
            GameObject go = CreateEmpty(name, parent);
            RawImage image = go.AddComponent<RawImage>();
            image.raycastTarget = false;
            return image;
        }

        public static T GetComponent<T>(string path, GameObject go) where T : Component
        {
            return GetComponent<T>(go, path);
        }
        public static T GetComponent<T>(GameObject go, string path = "") where T : Component
        {
            if (go == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(path))
            {
                return go.GetComponent<T>();
            }

            Transform tran = go.transform.Find(path);
            if (tran == null)
            {
                return null;
            }
            return tran.GetComponent<T>();
        }

        public static GameObject FindGetChildGameObject(GameObject go, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                return go;
            }
            Transform transform = go.transform;
            int len=transform.childCount;

            GameObject temp;
            for (int i = 0; i < len; i++)
            {
                Transform child=transform.GetChild(i);
                if (child.name == name)
                {
                    return child.gameObject;
                }
                temp=FindGetChildGameObject(child.gameObject, name);
                if (temp != null)
                {
                    return temp;
                }
            }
            return null;
        }

        public static GameObject FindGetChildGameObject(GameObject[] gos, string name = "")
        {
            int len = gos.Length;
            GameObject result = null;
            for (int i = 0; i < len; i++)
            {
                GameObject go = gos[i];
                result=FindGetChildGameObject(go, name);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        public static GameObject FindGetChildGameObject(List<GameObject> gos, string name = "")
        {
            int len = gos.Count;
            GameObject result = null;
            for (int i = 0; i < len; i++)
            {
                GameObject go = gos[i];
                result = FindGetChildGameObject(go, name);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        public static Text GetText(GameObject parent, string path="")
        {
            Text text= GetComponent<Text>(parent,path);
            text.raycastTarget = false;
            return text;
        }
      
        public static Button GetButton(GameObject parent, string path = "")
        {
            return GetComponent<Button>(parent, path);
        }

        public static Image GetImage(GameObject parent, string path = "")
        {
            return GetComponent<Image>(parent, path);
        }
        public static RawImage GetRawImage(GameObject parent, string path = "")
        {
            return GetComponent<RawImage>(parent, path);
        }


        private static Material shareGrayMaterial;
        public static Material CreatShareGrayMaterial()
        {
            if (shareGrayMaterial == null)
            {
                shareGrayMaterial = new Material(Shader.Find(shareGrayShaderPath));
            }
            return shareGrayMaterial;
        }

        public static Vector3 Local2Local(Transform fromParent, Vector3 fromVector3, Transform to)
        {
            Vector3 worldVector3 = fromParent.TransformPoint(fromVector3);
            return to.InverseTransformPoint(worldVector3);
        }

        public static Vector3 Local2Local(Transform from, Transform to)
        {
            return Local2Local(from.parent, from.localPosition, to);
        }

        public static Vector3 Global2Local(Vector3 worldVector3, Transform to)
        {
            return to.InverseTransformPoint(worldVector3);
        }

        private static List<RaycastResult> cacheRaycastResults = new List<RaycastResult>();
        public static bool IsPointerOverUI(Vector3 position, GameObject obj)
        {
            EventSystem eventSystem = EventSystem.current;
            GameObject clickObj;
            GraphicRaycaster ray = AbstractApp.GraphicRaycaster;
            PointerEventData pointData = new PointerEventData(eventSystem);
            pointData.position = position;
            pointData.pressPosition = position;
            ray.Raycast(pointData, cacheRaycastResults);
            if (cacheRaycastResults.Count == 0) return false;
            clickObj = cacheRaycastResults[0].gameObject;
            cacheRaycastResults.Clear();

            bool result = false;
            Transform parent = clickObj.transform;
            while (parent != null)
            {
                if (parent.gameObject == obj)
                {
                    result = true;
                    break;
                }
                else
                {
                    parent = parent.parent;
                }
            }
            return result;
        }
    }
}