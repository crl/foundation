using UnityEngine;

namespace foundation
{
    public abstract class AbstractBaseObject : FoundationBehaviour, ISkinable
    {
        /// <summary>
        /// 代理的皮肤对像(形像与逻辑分离原则)
        /// </summary>
        [SerializeField] protected GameObject _skin;

        protected virtual Transform skinParentTransform
        {
            get { return this.transform; }
        }

        protected virtual void Awake()
        {
        }


        public virtual void SetActive(bool value)
        {
            if (isDisposed)
            {
                return;
            }
            gameObject.SetActive(value);
        }

        private bool _renderable = true;

        public virtual bool renderable
        {
            get { return _renderable; }
            set
            {
                if (_renderable != value)
                {
                    _renderable = value;
                    RenderUtils.SetEnabledRecursive(gameObject, value);
                    this.simpleDispatch(EventX.RENDERABLE_CHANGE);
                }
            }
        }

        public virtual void lookAt(Vector3 worldPosition)
        {
            worldPosition.y = transform.position.y;
            transform.LookAt(worldPosition);
        }

        public virtual GameObject skin
        {
            set
            {
                if (_skin == value)
                {
                    return;
                }

                if (_skin != null)
                {
                    unbindComponents();
                    recycleSkin();
                }

                _skin = value;
                if (_skin != null)
                {
                    if (this.skinParentTransform != _skin)
                    {
                        _skin.transform.SetParent(this.skinParentTransform, false);
                    }

                    prebindComponents();
                    bindComponents();
                    postbindComponents();
                }
            }
            get { return _skin; }
        }

        protected virtual void bindUnitConfig(UnitCFG unit)
        {
        }

        protected virtual void prebindComponents()
        {
            RenderUtils.SetEnabledRecursive(gameObject, _renderable);
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

        protected virtual void recycleSkin()
        {
        }
    }
}