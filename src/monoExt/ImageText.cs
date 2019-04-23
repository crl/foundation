using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [AddComponentMenu("Lingyu/ImageText")]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class ImageText : MonoBehaviour, ICanbeReplacBehaviour
    {
        [HideInInspector]
        public UpkAniVO upkAniVo;

        public string title;
        public string message;

        private Transform image;
        private string _prefix;
        protected virtual void Awake()
        {
            AbstractApp.ReplacBehaviour(this);
            Show();
        }

        public void Show(string msg,string prefix=null)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = title;
            }
            _prefix = prefix;
            message = msg;
          
            Show();
        }

        public void Show()
        {
            if (upkAniVo == null || message == null || _prefix == null) 
            {
                return;
            }
            if (image == null)
            {
                if (transform.childCount > 0)
                {
                    image = transform.GetChild(0);
                }
                if (image == null)
                {
                    image = UIUtils.CreateImage("image0", gameObject).transform;
                }
            }
            List<Image> nums = gameObject.GetComponentsInChildren<Image>(true).ToList();
            if (nums.Count == 0) return;
            if (nums.Count < message.Length)
            {
                for (int i = nums.Count; i < message.Length; i++)
                {
                    GameObject temp = Instantiate(image.gameObject) as GameObject;
                    temp.transform.SetParent(gameObject.transform);
                    temp.transform.localScale = Vector3.one;
                    temp.GetComponent<RectTransform>().anchoredPosition =
                        image.GetComponent<RectTransform>().anchoredPosition;
                    temp.GetComponent<RectTransform>().localRotation =
                        image.GetComponent<RectTransform>().localRotation;
                    temp.GetComponent<RectTransform>().localPosition =
                        image.GetComponent<RectTransform>().localPosition;
                    nums.Add(temp.GetComponent<Image>());
                }
            }
            for (int i = 0; i < nums.Count; i++)
            {
                if (i < message.Length)
                {
                    nums[i].sprite = upkAniVo.GetSpriteByName(_prefix + message[i]);
                    nums[i].gameObject.SetActive(true);
                    nums[i].transform.SetSiblingIndex(i);
                }
                else
                {
                    nums[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
