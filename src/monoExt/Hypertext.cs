using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace foundation
{
    [AddComponentMenu("Lingyu/Hypertext")]
    public class Hypertext : Text, IPointerClickHandler,ICanbeReplacBehaviour
    {
        /// <summary>
        /// 图片池
        /// </summary>
        private readonly List<AniTag> m_ImagesPool = new List<AniTag>();

        /// <summary>
        /// 图片的最后一个顶点的索引
        /// </summary>
        private readonly List<int> m_ImagesVertexIndex = new List<int>();

        /// <summary>
        /// 正则取出所需要的属性
        /// </summary>
        private static readonly Regex s_Regex =
            new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) exts=(\d*\.?\d+%?) />", RegexOptions.Singleline);

        /// <summary>
        /// 超链接信息列表
        /// </summary>
        private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();
        /// <summary>
        /// 文本构造器
        /// </summary>
        private static readonly StringBuilder s_TextBuilder = new StringBuilder();

        /// <summary>
        /// 超链接正则
        /// </summary>
        private static readonly Regex s_HrefRegex =
            new Regex(@"<a href\s*=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

        public Action<string> onHrefClick;

        public static Action<string> defaultOnHrefClick;

        /// <summary>
        /// 点击除超链接外其他区域
        /// </summary>
        public Action<Hypertext> onTextClickWithoutHref;

        /// <summary>
        /// 表情图片整体上下移动
        /// </summary>
        public float spriteOffsetY = 0;

        private bool _hrefBoundsChangeFlag;
        private List<Vector3> vertices = new List<Vector3>();

        protected override void Awake()
        {
            AbstractApp.ReplacBehaviour(this);
            base.Awake();
        }
        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            UpdateQuadImage();
        }

        /// <summary>
        /// 解析完最终的文本
        /// </summary>
        private string m_OutputText;

        protected void UpdateQuadImage(bool b = true)
        {
            m_OutputText = GetOutputText();
            m_ImagesVertexIndex.Clear();
            foreach (Match match in s_Regex.Matches(m_OutputText))
            {
                int picIndex = match.Index;
                int endIndex = picIndex*4 + 3;
                m_ImagesVertexIndex.Add(endIndex);

                m_ImagesPool.RemoveAll(image => image == null);
                if (m_ImagesPool.Count == 0)
                {
                    GetComponentsInChildren<AniTag>(m_ImagesPool);
                }
                if (m_ImagesVertexIndex.Count > m_ImagesPool.Count)
                {
                    DefaultControls.Resources resources = new DefaultControls.Resources();
                    GameObject go = DefaultControls.CreateImage(resources);
                    go.layer = gameObject.layer;
                    RectTransform rt = go.transform as RectTransform;
                    if (rt)
                    {
                        rt.SetParent(rectTransform);
                        rt.localPosition = Vector3.zero;
                        rt.localRotation = Quaternion.identity;
                        rt.localScale = Vector3.one;
                    }
                    AniTag tag = go.AddComponent<AniTag>();
                    tag.image = go.GetComponent<Image>();
                    tag.image.raycastTarget = false;
                    m_ImagesPool.Add(tag);
                }

                string spriteName = match.Groups[1].Value;
                float size = float.Parse(match.Groups[2].Value);
                float width = float.Parse(match.Groups[3].Value);
                float eSize = float.Parse(match.Groups[4].Value);
                if (eSize>0)
                {
                    size += eSize;
                }

                AniTag aniImg = m_ImagesPool[m_ImagesVertexIndex.Count - 1];
                Image img = aniImg.image;
                RectTransform imgRect = img.GetComponent<RectTransform>();
                imgRect.sizeDelta = new Vector2(size, size);
                if (b)
                {
                    aniImg.load(spriteName);
                }
                aniImg.toggle(true);
            }

            for (int i = m_ImagesVertexIndex.Count; i < m_ImagesPool.Count; i++)
            {
                if (m_ImagesPool[i] != null)
                {
                    m_ImagesPool[i].toggle(false);
                }
            }
        }


        public override float preferredHeight
        {
            get
            {
                float _preferredHeight = this.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_OutputText, this.GetGenerationSettings(new Vector2(this.GetPixelAdjustedRect().size.x, 0.0f))) / this.pixelsPerUnit;
                return _preferredHeight;
            }
        }

        public override float preferredWidth {
            get
            {
                return this.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_OutputText, this.GetGenerationSettings(Vector2.zero)) / this.pixelsPerUnit;
            }
        }

#region 这部分代码来自Text,复制过来更方便的获取顶点数据

        private readonly UIVertex[] m_TempVerts = new UIVertex[4];
        private void base_OnPopulateMesh(VertexHelper toFill)
        {
            if ((UnityEngine.Object)this.font == (UnityEngine.Object)null)
                return;
            this.m_DisableFontTextureRebuiltCallback = true;
            this.cachedTextGenerator.PopulateWithErrors(this.text, this.GetGenerationSettings(this.rectTransform.rect.size), this.gameObject);
            IList<UIVertex> verts = this.cachedTextGenerator.verts;
            float num1 = 1f / this.pixelsPerUnit;
            int num2 = verts.Count - 4;
            Vector2 point = new Vector2(verts[0].position.x, verts[0].position.y) * num1;
            Vector2 vector2 = this.PixelAdjustPoint(point) - point;
            toFill.Clear();
            if (vector2 != Vector2.zero)
            {
                for (int index1 = 0; index1 < num2; ++index1)
                {
                    int index2 = index1 & 3;
                    this.m_TempVerts[index2] = verts[index1];
                    this.m_TempVerts[index2].position *= num1;
                    this.m_TempVerts[index2].position.x += vector2.x;
                    this.m_TempVerts[index2].position.y += vector2.y;
                    //增加顶点
                    vertices.Add(this.m_TempVerts[index2].position);

                    if (index2 == 3)
                    {
                        toFill.AddUIVertexQuad(this.m_TempVerts);
                    }
                }
            }
            else
            {
                for (int index1 = 0; index1 < num2; ++index1)
                {
                    int index2 = index1 & 3;
                    this.m_TempVerts[index2] = verts[index1];
                    this.m_TempVerts[index2].position *= num1;
                    //增加顶点
                    vertices.Add(this.m_TempVerts[index2].position);

                    if (index2 == 3)
                    {
                        toFill.AddUIVertexQuad(this.m_TempVerts);
                    }
                }
            }
            this.m_DisableFontTextureRebuiltCallback = false;
        }
#endregion

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            string orignText = m_Text;
            m_Text = m_OutputText;

            vertices.Clear();

            base_OnPopulateMesh(toFill);
            
            m_Text = orignText;

            UIVertex vert = new UIVertex();
            for (int i = 0; i < m_ImagesVertexIndex.Count; i++)
            {
                int endIndex = m_ImagesVertexIndex[i];
                RectTransform rt = m_ImagesPool[i].image.rectTransform;
                Vector2 size = rt.sizeDelta;
                if (endIndex < toFill.currentVertCount)
                {
                    toFill.PopulateUIVertex(ref vert, endIndex);

                    Vector2 newPosition = new Vector2();
                    newPosition.x= vert.position.x+rectTransform.sizeDelta.x * (rectTransform.pivot.x - 0.5f)+ size.x * 0.5f;
                    newPosition.y= vert.position.y +rectTransform.sizeDelta.y * (rectTransform.pivot.y - 0.5f) + +size.y * 0.5f + spriteOffsetY;
                    rt.anchoredPosition=newPosition;

                    // 抹掉左下角的小黑点
                    toFill.PopulateUIVertex(ref vert, endIndex - 3);
                    Vector3 pos = vert.position;
                    for (int j = endIndex, m = endIndex - 3; j > m; j--)
                    {
                        toFill.PopulateUIVertex(ref vert, endIndex);
                        vert.position = pos;
                        toFill.SetUIVertex(vert, j);
                        //顶点位置修改
                        vertices[j] = pos;
                    }
                }
            }

            if (m_ImagesVertexIndex.Count != 0)
            {
                m_ImagesVertexIndex.Clear();
            }
            
            
            _hrefBoundsChangeFlag = true;
        }

        

        /// <summary>
        /// 获取超链接解析后的最后输出文本
        /// </summary>
        /// <returns></returns>
        protected string GetOutputText()
        {
            s_TextBuilder.Length = 0;
            m_HrefInfos.Clear();
            int indexText = 0;
            foreach (Match match in s_HrefRegex.Matches(text))
            {
                s_TextBuilder.Append(text.Substring(indexText, match.Index - indexText));
                Group group = match.Groups[1];
                HrefInfo hrefInfo = new HrefInfo
                {
                    startIndex = s_TextBuilder.Length*4, // 超链接里的文本起始顶点索引
                    endIndex = (s_TextBuilder.Length + match.Groups[2].Length - 1)*4 + 3,
                    name = group.Value
                };
                m_HrefInfos.Add(hrefInfo);
                s_TextBuilder.Append(match.Groups[2].Value);
                indexText = match.Index + match.Length;
            }
            s_TextBuilder.Append(text.Substring(indexText, text.Length - indexText));
            return s_TextBuilder.ToString();
        }

        private void calculateHrefBounds()
        {
            if (_hrefBoundsChangeFlag)
            {
                _hrefBoundsChangeFlag = false;

                int verticesCount = vertices.Count;

                // 处理超链接包围框
                foreach (var hrefInfo in m_HrefInfos)
                {
                    hrefInfo.boxes.Clear();
                    if (hrefInfo.startIndex >= verticesCount)
                    {
                        continue;
                    }

                    // 将超链接里面的文本顶点索引坐标加入到包围框
                    Vector3 pos = vertices[hrefInfo.startIndex];
                   
                    Bounds bounds = new Bounds(pos, Vector3.zero);
                    for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++)
                    {
                        if (i >= verticesCount)
                        {
                            break;
                        }

                        pos = vertices[i];
                        if (pos.x < bounds.min.x) // 换行重新添加包围框
                        {
                            hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                            bounds = new Bounds(pos, Vector3.zero);
                        }
                        else
                        {
                            bounds.Encapsulate(pos); // 扩展包围框
                        }
                    }
                    hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                }
            }
        }

        /// <summary>
        /// 点击事件检测是否点击到超链接文本
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out point);

            // 处理超链接包围框
            calculateHrefBounds();

            foreach (HrefInfo hrefInfo in m_HrefInfos)
            {
                List<Rect> boxes = hrefInfo.boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(point))
                    {
                        string key = hrefInfo.name.Substring(1, hrefInfo.name.Length - 2);  //去掉返回key中括号
                        if (onHrefClick != null)
                        {
                            onHrefClick(key);
                        }
                        else
                        {
                            if (defaultOnHrefClick != null)
                            {
                                defaultOnHrefClick(key);
                            }
                        }
                        return;
                    }
                }
            }
            if (onTextClickWithoutHref != null)
            {
                onTextClickWithoutHref(this);
            }
        }

        /// <summary>
        /// 超链接信息类
        /// </summary>
        private class HrefInfo
        {
            public int startIndex;

            public int endIndex;

            public string name;

            public readonly List<Rect> boxes = new List<Rect>();
        }
    }

    public class AniTag : MonoBehaviour
    {
        public Image image;

        private ImageAnimation anim;

        private static ASDictionary<string> upkList = new ASDictionary<string>();
        public static ASDictionary<Sprite> spriteList = new ASDictionary<Sprite>();

        public static ASDictionary<string> GetUpkList()
        {
            return upkList;
        }

        public static void addUpkMap(string key, string path)
        {
            upkList.Add(key, path);
        }

        public static void addSprite(string key, Sprite sprite)
        {
            spriteList.Add(key, sprite);
        }

        private void Start()
        {
            image = GetComponent<Image>();
        }

        public void toggle(bool v)
        {
            if (image != null)
            {
                if(gameObject.activeSelf!=v)gameObject.SetActive(v);
            }
        }

        public void load(string spriteName)
        {
            if (anim != null)
            {
                anim.stop();
            }

            string path;
            if (upkList.TryGetValue(spriteName, out path))
            {
                if (anim == null)
                {
                    anim = gameObject.AddComponent<ImageAnimation>();
                }
                anim.load(path);
                anim.play(true);
                return;
            }

            Sprite sprite;
            if (spriteList.TryGetValue(spriteName, out sprite))
            {
                image.sprite = sprite;
                return;
            }

            image.sprite = Resources.Load<Sprite>(spriteName);
        }
    }
}