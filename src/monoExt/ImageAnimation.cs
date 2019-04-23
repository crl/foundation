using System;
using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [AddComponentMenu("Lingyu/ImageAnimation")]
    [RequireComponent(typeof (Image))]
    public class ImageAnimation : MonoBehaviour,ICanbeReplacBehaviour
    {
        [HideInInspector] public Action completeHandle;

        public UpkAniVO upkAniVo;
        public bool isPlaying = false;
        public bool isLoop = true;
        public bool autoNativeSize = false;
        private float timef;

        private Image image;

        protected virtual void Awake()
        {
            AbstractApp.ReplacBehaviour(this);
            image = GetComponent<Image>();
        }

        [ContextMenu("Test")]
        public virtual void test()
        {
            if (upkAniVo != null)
            {
                image = GetComponent<Image>();
                image.sprite = upkAniVo.keys[0].sprite;
            }
        }

        protected void Update()
        {
            if (upkAniVo == null || !isPlaying)
            {
                return;
            }
            timef += Time.deltaTime;
            int frame = Mathf.FloorToInt(timef*upkAniVo.fps);
            if (frame > upkAniVo.keys.Count - 1)
            {
                if (isLoop == false)
                {
                    isPlaying = false;
                    timef = 0;
                    if (completeHandle == null)
                    {
                        this.completeHandle();
                    }
                    return;
                }

                frame %= upkAniVo.keys.Count;
            }
            image.sprite = upkAniVo.keys[frame].sprite;
            if (autoNativeSize)
            {
                image.SetNativeSize();
            }
        }

        public void play(bool reset = false)
        {
            isPlaying = true;

            if (reset)
            {
                timef = 0;
            }
        }

        public void stop()
        {
            isPlaying = false;
        }

        private AssetResource resource;

        private string uri;
        public void load(string uri)
        {
            if (this.uri == uri)
            {
                return;
            }

            this.uri = uri;

            if (string.IsNullOrEmpty(uri))
            {
                return;
            }

            string url = getURL(uri);
            if (resource != null)
            {
                resource.release();
                AssetsManager.bindEventHandle(resource, resourceHandle, false);
            }

            resource = AssetsManager.getResource(url, LoaderXDataType.ASSETBUNDLE);
            resource.retain();
            AssetsManager.bindEventHandle(resource, resourceHandle);
            resource.load();
        }

        protected virtual string getURL(string uri)
        {
            return PathDefine.uiPath + "ui/" + uri + PathDefine.U3D;
        }

        private void resourceHandle(EventX e)
        {
            AssetResource resource = e.target as AssetResource;
            AssetsManager.bindEventHandle(resource, resourceHandle, false);

            if (e.type != EventX.COMPLETE)
            {
                return;
            }
            UpkAniVO o =  resource.getMainAsset() as UpkAniVO;
            if (null == o)
            {
                return;
            }
            upkAniVo = o;
        }
    }
}