using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [RequireComponent(typeof(Image))]
    public class UpkIconView : MonoBehaviour
    {
        public UpkAniVO upkAniVo;
        public string title;
        public string message;
        public bool autoShow = false;
        private string _prefix;
        private Image image;
        protected virtual void Awake()
        {
            if (autoShow)
            {
                doShow();
            }
        }

        public void show(string msg, string prefix = null)
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            if (string.IsNullOrEmpty(prefix))
            {
                prefix = title;
            }
            _prefix = prefix;
            message = msg;

            doShow();
        }

        protected virtual void doShow()
        {
            if (upkAniVo == null || message == null || _prefix == null || image==null)
            {
                return;
            }

            Sprite sp=upkAniVo.GetSpriteByName(_prefix + message);
            if (sp == null)
            {
                image.enabled = false;
            }
            else if (image.enabled == false)
            {
                image.enabled = true;
            }

            image.sprite = sp;
        }

        public void SetNativeSize()
        {
            if (image != null) image.SetNativeSize();
        }

        public void resetSize(int width = -1, int height = -1)
        {
            UIUtils.SetSize(gameObject, width, height);
        }
    }
}