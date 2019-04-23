using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    public class ClayButton : SkinBase
    {
        /// <summary>
        /// 关闭音效
        /// </summary>
        public static string CloseSound = "";

        public static string DEFAULT_CLICK_SOUND_NAME;
        public Text label;
        public Image image;
        private Button _button;
        private Outline[] outlines = new Outline[] { };

        public RawImage rawImage;
        public ImageAnimation upkAnimation;
        public string clickAnimName;
        public string clickSoundName;
        public Image disabledImage;

        /// <summary>
        /// 按住自动点击
        /// </summary>
        protected bool _autoClick = false;

        /// <summary>
        /// 点击间隔
        /// </summary>
        public float autoClickDelay = 0.048f;

        /// <summary>
        /// 变灰色时候是否把label也变灰
        /// </summary>
        public bool isChangeLabelWhenGray = true;

        private string _lableName;
        private Color _labelColor;

        private Vector3 defaultScale=Vector3.one;
        public ClayButton(GameObject skin, string lableName = "text")
        {
            this._lableName = lableName;
            this.skin = skin;
            clickSoundName = DEFAULT_CLICK_SOUND_NAME;
        }

        private bool _tweenScale = true;

        public bool tweenScale
        {
            get { return _tweenScale; }
            set
            {
                _tweenScale = value;
                bindTweenScale(value);
            }
        }
        protected override void bindComponents()
        {
            if (string.IsNullOrEmpty(_lableName) == false)
            {
                label = getText(_lableName);
                if (label == null)
                {
                    label = getText("Text");
                }
                if (label != null)
                {
                    _labelColor = label.color;
                    outlines = label.gameObject.GetComponents<Outline>();
                }
            }

            rawImage = getRawImage("");
            image = getImage("");

            disabledImage = getImage("disabledImage");

            if (disabledImage != null)
            {
                disabledImage.enabled = false;
            }

            defaultScale = _skin.transform.localScale;
            _button = getButton("");
            if (_button == null)
            {
                _button = _skin.AddComponent<Button>();
            }
            _button.onClick.AddListener(clickHandle);


            if (_autoClick)
            {
                bindAutoClick(_autoClick);
            }
            if (_tweenScale)
            {
                bindTweenScale(_tweenScale);
            }
        }


        protected virtual void bindTweenScale(bool value)
        {
            if (_skin == null)
            {
                return;
            }

            MouseScaleEffectMono tween = _skin.GetComponent<MouseScaleEffectMono>();
            if (value)
            {
                if (tween == null)
                {
                    tween = skin.AddComponent<MouseScaleEffectMono>();
                }
                else
                {
                    tween.enabled = true;
                }
                tween.defaultScale = defaultScale;
            }
            else if (tween != null)
            {
                tween.enabled = false;
            }
        }
        protected virtual void bindAutoClick(bool value)
        {
            if (_skin == null)
            {
                return;
            }
            ASEventTrigger trriger = EventDispatcher.Get(_skin);
            trriger.mouseEnterEnabled = value;
            if (value)
            {
                _button.onClick.RemoveListener(clickHandle);
                trriger.addEventListener(MouseEventX.MOUSE_DOWN, mouseEventHandle);
                trriger.addEventListener(MouseEventX.MOUSE_ENTER, mouseEventHandle);
                trriger.addEventListener(MouseEventX.MOUSE_UP, mouseEventHandle);
            }
            else
            {
                _button.onClick.AddListener(clickHandle);
                trriger.removeEventListener(MouseEventX.MOUSE_DOWN, mouseEventHandle);
                trriger.removeEventListener(MouseEventX.MOUSE_ENTER, mouseEventHandle);
                trriger.removeEventListener(MouseEventX.MOUSE_UP, mouseEventHandle);
            }
        }

        public Button button
        {
            get { return _button; }
        }

        public bool stopImmediatePropagation { get; set; }

        private void clickHandle()
        {
            if (_enabled == false)
            {
                return;
            }
            playUpk(clickAnimName);
            playSound(clickSoundName);

            if (hasEventListener(EventX.CLICK))
            {
                EventX e = EventX.FromPool(EventX.CLICK, null, false);
                if (stopImmediatePropagation)
                {
                    e.stopImmediatePropagation();
                }
                bool b = dispatchEvent(e);
                EventX.ToPool(e);
            }
        }

        public virtual void playSound(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            AbstractApp.soundsManager.playUISoundOnce(value);
        }

        public virtual void playUpk(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (upkAnimation == null)
            {
                Image image = UIUtils.CreateImage("anim", this.skin);
                upkAnimation = image.gameObject.AddComponent<ImageAnimation>();
                upkAnimation.autoNativeSize = true;
            }
            upkAnimation.load(name);
            upkAnimation.play(true);
        }

        protected override void doEnabled()
        {
            if (image != null)
            {
                if (disabledImage != null)
                {
                    disabledImage.enabled = !_enabled;
                }
                else
                {
                    image.material = UIUtils.CreatShareGrayMaterial();
                    if (_enabled)
                    {
                        image.color = Color.white;
                        //image.color = ColorUtils.ToColor(0x333333FF);
                    }
                    else
                    {
                        image.color = new Color(0, 1, 1, 1);
                        //image.color = Color.white;
                    }
                }
            }
            else
            {
                if (rawImage != null)
                {
                    if (disabledImage != null)
                    {
                        disabledImage.enabled = !_enabled;
                    }
                    else
                    {
                        rawImage.material = UIUtils.CreatShareGrayMaterial();
                        if (_enabled)
                        {
                            rawImage.color = Color.white;
                            //rawImage.color = ColorUtils.ToColor(0x333333FF);
                        }
                        else
                        {
                            rawImage.color = new Color(0,1,1,1);
                            //rawImage.color = Color.white;
                        }
                    }
                }
            }

            if (button != null)
            {
                button.enabled = enabled;
            }

            if (label != null && isChangeLabelWhenGray==true)
            {
                foreach (var item in outlines)
                {
                    item.enabled = _enabled;
                }
                if (_enabled)
                {
                    label.color = _labelColor;
                }
                else
                {
                    label.color = Color.white;
                }
            }
        }

        private string _text="";
        public string text
        {
            get { return _text; }
            set
            {
                _text = value;

                if (label != null)
                {
                    label.text = value;
                }
            }
        }

        /// <summary>
        /// 按住自动点击
        /// </summary>
        public bool autoClick
        {
            get { return _autoClick; }
            set
            {
                _autoClick = value;
                bindAutoClick(value);
            }
        }

        private float _autoClickStartTime;
        private bool _autoClicked;

        private void mouseEventHandle(EventX e)
        {
            float now = Time.realtimeSinceStartup;
            if (e.type == MouseEventX.MOUSE_DOWN)
            {
                //0.3秒后开始检测自动点击
                _autoClickStartTime = now + 0.15f;
                _autoClicked = false;
            }
            else if (e.type == MouseEventX.MOUSE_ENTER)
            {
                float nextClickTime = _autoClickStartTime + autoClickDelay;
                if (now >= nextClickTime)
                {
                    _autoClickStartTime = now;
                    _autoClicked = true;
                    this.simpleDispatch(EventX.CLICK);
                }
            }
            else
            {
                if (!_autoClicked)
                {
                    playUpk(clickAnimName);
                    //没有被点击过，则弹起的时候触发点击
                    this.simpleDispatch(EventX.CLICK);
                }
            }
        }
    }
}