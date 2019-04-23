using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    public class SkinBase : EventDispatcher, ISkinable, IDataRenderer, IEventInterester, IFocus,IDisposable
    {
        protected object _data;
        protected GameObject _skin;
        private RectTransform _rectTransform;
        protected Vector3 _position = Vector3.zero;
        protected Vector3 _scale = Vector3.one;
        protected string _name;

        /// <summary>
        /// 被其它销毁时，用于存储以前是否有设置过皮肤
        /// </summary>
        protected bool _hasSkin=false;
        public virtual object data
        {
            get { return _data; }

            set
            {
                _data = value;
                doData();
            }
        }

        public string name
        {
            get
            {
                if (_name == null && _skin != null)
                {
                    _name = _skin.name;
                }

                return _name;
            }
            set { _name = value; }
        }

        public Vector3 positionXY
        {
            get
            {
                if (_skin == null)
                {
                    return _position;
                }

                return _skin.transform.localPosition;
            }
            set
            {
                if (_skin == null)
                {
                    _position = value;
                    return;
                }
                _skin.transform.localPosition = value;
            }
        }

        public Vector3 scaleXY
        {
            get
            {
                if (_skin == null)
                {
                    return _scale;
                }
                return _skin.transform.localScale;
            }
            set
            {
                if (_skin == null)
                {
                    _scale = value;
                    return;
                }
                _skin.transform.localScale = value;
            }
        }


        public Vector2 getSize()
        {
            RectTransform rectTransform = _skin.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform.sizeDelta;
            }

            return _skin.transform.localScale;
        }

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = _skin.GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }

        /// <summary>
        /// 强制刷新;
        /// </summary>
        /// refr
        public virtual void refresh()
        {
            doData();
        }

        protected virtual void doData()
        {
           
        }

        protected bool autoDefaultSize = false;

        public virtual GameObject skin
        {
            get { return _skin; }
            set
            {
                if (_skin != null)
                {
                    bindSkinEvent(false);
                    unbindComponents();
                }

                _skin = value;

                if (_skin != null)
                {
                    _hasSkin = true;

                    if (_skin.activeSelf != _isActive)
                    {
                        _skin.SetActive(_isActive);
                    }

                    prebindComponents();
                    bindComponents();
                    bindSkinEvent(true);
                    postbindComponents();

                    if (_isActive && _skin.activeInHierarchy)
                    {
                        stageHandle(new EventX(EventX.ADDED_TO_STAGE));
                    }
                }
                else
                {
                    _hasSkin = false;
                }

                this.simpleDispatch(EventX.SET_SKIN);
            }
        }

        protected virtual void bindSkinEvent(bool v)
        {
            if (v)
            {
                _skin.addEventListener(EventX.ADDED_TO_STAGE, stageHandle);
                _skin.addEventListener(EventX.REMOVED_FROM_STAGE, stageHandle);
                _skin.addEventListener(EventX.DESTOTY, skinDestoryHandle);
            }
            else
            {
                _skin.removeEventListener(EventX.ADDED_TO_STAGE, stageHandle);
                _skin.removeEventListener(EventX.REMOVED_FROM_STAGE, stageHandle);
                _skin.removeEventListener(EventX.DESTOTY, skinDestoryHandle);
            }
        }

        protected bool _enabled = true;
        public bool enabled
        {
            set
            {
                _enabled = value;
                doEnabled();
            }
            get { return _enabled; }
        }

        protected virtual void doEnabled()
        {
        }


        protected bool _isActive = true;
        public virtual void SetActive(bool v)
        {
            _isActive = v;
            if (_skin != null && _isActive!=_skin.activeSelf)
            {
                _skin.SetActive(v);
            }
        }

        public virtual void SetFocus(bool v)
        {
            FocusGameObject mono = _skin.GetComponent<FocusGameObject>();
            if (mono != null)
            {
                mono.setFocus(v);
            }
        }

        public bool activeInHierarchy
        {
            get
            {
                if (_skin != null)
                {
                    return _skin.activeInHierarchy;
                }
                return false;
            }
        }

        protected virtual void clickTween()
        {
            if (_skin == null)
            {
                return;
            }
            TweenScale.Play(_skin, 0.1f, Vector3.one * 0.95f);
            CallLater.Add(() => TweenScale.Play(_skin, 0.1f, Vector3.one), 0.1f);
        }


        public bool isActive
        {
            get
            {
                if (_skin != null)
                {
                    return _skin.activeSelf;
                }

                return _isActive;
            }
        }

        protected virtual void stageHandle(EventX e)
        {
            if (e.type == EventX.ADDED_TO_STAGE)
            {
                preAwaken();
            }
            else if (e.type == EventX.REMOVED_FROM_STAGE)
            {
                preSleep();
            }

            this.dispatchEvent(e);
        }

        protected virtual void preAwaken()
        {
            awaken();
            updateView();
        }

        protected virtual void awaken()
        {
        }

        protected virtual void updateView(EventX e=null)
        {
            
        }

        protected virtual void preSleep()
        {
            sleep();
        }

        protected virtual void sleep()
        {

        }

        protected virtual void skinDestoryHandle(EventX e)
        {
            _hasSkin = false;
            this.onSkinDestroy();
            this.dispatchEvent(e);
        }

        protected virtual void prebindComponents()
        {
            if (autoDefaultSize)
            {
                _skin.transform.localPosition = Vector3.zero;
                _skin.transform.localRotation = Quaternion.identity;
                _skin.transform.localScale = Vector3.one;
            }

            if (_position != Vector3.zero)
            {
                _skin.transform.localPosition = _position;
            }

            if (_scale != Vector3.one)
            {
                _skin.transform.localScale = _scale;
            }
        }

        protected virtual void bindComponents()
        {
        }

        protected virtual void postbindComponents()
        {
  
        }

        protected virtual void unbindComponents()
        {
   
        }

        protected virtual void onSkinDestroy()
        {

        }

        public Text getText(string name, GameObject parent = null)
        {
            return getComponent<Text>(name, parent);
        }

        public RawImage getRawImage(string name, GameObject parent = null)
        {
            return getComponent<RawImage>(name, parent);
        }

        public Button getButton(string name, GameObject parent = null)
        {
            return getComponent<Button>(name, parent);
        }

        public Image getImage(string name, GameObject parent = null)
        {
            return getComponent<Image>(name, parent);
        }

        public T getComponent<T>(string path = "", GameObject go = null) where T : Component
        {
            if (go == null)
            {
                go = _skin;
            }
            return UIUtils.GetComponent<T>(go, path);
        }

        public GameObject getGameObject(string name, GameObject go = null)
        {
            if (go == null)
            {
                go = _skin;
            }
            Transform transform = go.transform.Find(name);
            if (transform != null)
            {
                return transform.gameObject;
            }

            return null;
        }

        private Dictionary<InjectEventType,Dictionary<string, Action<EventX>>> _eventInterests;
        public Dictionary<string, Action<EventX>> getEventInterests(InjectEventType type)
        {
            if (_eventInterests == null)
            {
                _eventInterests = new Dictionary<InjectEventType, Dictionary<string, Action<EventX>>>();
                MVCEventAttribute.CollectionEventInterests(this, _eventInterests);
            }
            Dictionary<string, Action<EventX>> e;
            if (_eventInterests.TryGetValue(type, out e) == false)
            {
                e = new Dictionary<string, Action<EventX>>();
                _eventInterests.Add(type, e);
            }
            return e;
        }
    }
}