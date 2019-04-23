using foundation;
using System.Collections.Generic;
using UnityEngine;

namespace gameSDK
{
    public class BaseSoundsManager : FoundationBehaviour
    {
        private const string BMG = "bmg";
        protected float _soundValue = 1.0f;
        /// <summary>
        /// 同一时间不能出现两次声音
        /// </summary>
        protected HashSet<string> _soundsOnce = new HashSet<string>();

        /// <summary>
        /// 所有声音的库
        /// </summary>
        private Dictionary<string, SoundClip> _soundsDictionary = new Dictionary<string, SoundClip>();

        private BaseAppSetting appSetting;

        protected virtual void Start()
        {
            appSetting = BaseAppSetting.GetInstance();
            this.enabled = appSetting.soundEnabled;
            this.soundValue = appSetting.soundPercent;
        }

        public virtual void stopSound(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            SoundClip soundClip;
            if (_soundsDictionary.TryGetValue(name, out soundClip))
            {
                soundClip.Stop();

                _soundsOnce.Remove(name);
            }
        }

        /// <summary>
        /// 声音实例
        /// </summary>
        //private Stack<GameObject> pools = new Stack<GameObject>();

        private bool _enabled = true;

        new public bool enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
            }
        }

        public float soundValue
        {
            get
            {
                return _soundValue;
            }
            set
            {
                _soundValue = value;
            }
        }

        protected bool _musicEnable = true;

        public bool musicEnable
        {
            get { return _musicEnable; }
            set
            {
                _musicEnable = value;
                refreash();
            }
        }

        protected float _musicValue;
        public float musicValue
        {
            get { return _musicValue; }
            set
            {
                _musicValue = value;
                refreash();
            }
        }

        protected virtual void refreash()
        {
            SoundClip soundClip;
            if (_soundsDictionary.TryGetValue(BMG, out soundClip))
            {
                if (soundClip == null)
                {
                    _soundsDictionary.Remove(BMG);
                    return;
                }

                if (soundClip.isPlaying)
                {
                    soundClip.soundValue = _musicValue;
                }

                if (_musicEnable)
                {
                    soundClip.Play();
                }
                else
                {
                    soundClip.Stop();
                }
            }
        }

        /// <summary>
        /// 播放ui音效（路径区别）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isFroce"></param>
        public void playUISound(string name, bool isFroce = false)
        {
            playSound(name, isFroce,true);
        }
        /// <summary>
        /// 单次播放ui音效（路径区别）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isFroce"></param>
        public void playUISoundOnce(string name, bool isFroce = false)
        {
            playSoundOnce(name,isFroce, true);
        }

        /// <summary>
        /// 单次播放的声音
        /// </summary>
        /// <param name="name"></param>
        public void playSoundOnce(string name,bool isFroce=false, bool isUI = false)
        {
            if (_enabled == false)
            {
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            if (_soundsOnce.Contains(name) && isFroce==false)
            {
                return;
            }
            _soundsOnce.Add(name);
            playSound(name, isFroce,isUI);
        }

        /// <summary>
        /// 即刻播放声音
        /// </summary>
        /// <param name="name"></param>
        public SoundClip playSound(string name,bool isFroce=false, bool isUI = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            SoundClip soundClip = null;
            if (_soundsDictionary.TryGetValue(name, out soundClip))
            {
                soundClip.soundValue = soundValue;
                if (soundClip.isPlaying == false)
                {
                    soundClip.Play();
                }
                else if (isFroce)
                {
                    soundClip.time = 0f;
                }
                return soundClip;
            }

            soundClip = getSoundInstance();
            _soundsDictionary.Add(name, soundClip);
         
            soundClip.soundValue = soundValue;
            soundClip.name = name;
            soundClip.addEventListener(EventX.COMPLETE, completeHandle);

            string url = getURL(name,isUI);
            soundClip.load(url);

            return soundClip;
        }

        public virtual string getURL(string uri, bool isUI = false)
        {
            string url = PathDefine.soundPath + "sound/" + uri + PathDefine.U3D;
            //todo 让子类来重写
            if (isUI)
            {
                url = PathDefine.uiPath + "ui/" + uri + PathDefine.U3D;
            }
            return url;
        }

        protected virtual SoundClip getSoundInstance()
        {
            GameObject soundItem = new GameObject("soundItem");
            soundItem.AddComponent<AudioSource>();
            soundItem.transform.SetParent(AbstractApp.SoundContainer.transform, true);
            return soundItem.AddComponent<SoundClip>();
        }

        protected void completeHandle(EventX e)
        {
            MonoEventDispatcher soundClip = (MonoEventDispatcher)e.target;
            string name = soundClip.name;
            _soundsOnce.Remove(name);
            soundClip.SetActive(false);
        }

       
        public void resetBMG(AudioSource audioSource)
        {
            SoundClip soundClip = null;
            if (_soundsDictionary.TryGetValue(BMG, out soundClip))
            {
                soundClip.Stop();
            }

            if (audioSource == null)
            {
                return;
            }
            soundClip=audioSource.GetComponent<SoundClip>();
            if (soundClip == null)
            {
                soundClip=audioSource.gameObject.AddComponent<SoundClip>();
            }

            audioSource.loop = true;
            soundClip.loop = true;
            _soundsDictionary[BMG] = soundClip;
            
            musicValue = appSetting.musicPercent;
            musicEnable = appSetting.musicEnabled;
        }

    }
}