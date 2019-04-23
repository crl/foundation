using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace foundation
{
    public class LinkButton : MonoBehaviour
    {
        private Text linkText;

        void Awake()
        {
            linkText = transform.Find("Text").GetComponent<Text>();

        }
        void Start()
        {
            CreateLink(linkText, onButtonClick);
        }

        public void CreateLink(Text text, UnityEngine.Events.UnityAction onClickBtn)
        {
            if (text == null)
                return;

            //克隆Text，获得相同的属性
            Text underline = Instantiate(text) as Text;
            underline.name = "Underline";

            underline.transform.SetParent(text.transform);
            RectTransform rt = underline.rectTransform;

            //设置下划线坐标和位置
            rt.anchoredPosition3D = Vector3.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;

            underline.text = "_";
            float perlineWidth = underline.preferredWidth;      //单个下划线宽度
            //Debug.Log(perlineWidth);

            float width = text.preferredWidth;
            //Debug.Log(width);
            int lineCount = (int)Mathf.Round(width / perlineWidth);
            //Debug.Log(lineCount);
            for (int i = 1; i < lineCount; i++)
            {
                underline.text += "_";
            }

            var btn = text.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(onClickBtn);
        }

        //点击响应
        void onButtonClick()
        {
            Debug.Log("onClick");
        }
    }
}