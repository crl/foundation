using System;
using UnityEngine;

namespace foundation
{
    public class BaseAppSetting
    {
        private const string PREFIX = "AppSetting_";

        private ASDictionary<string,object> dic=new ASDictionary();
        private static BaseAppSetting ins;
        public static BaseAppSetting GetInstance()
        {
            if (ins == null)
            {
                ins = new BaseAppSetting();
            }
            return ins;
        }

        private bool _musicEnabled = true;
        protected float _musicPercent = 1.0f;

        private bool _soundEnabled = true;
        private float _soundPercent = 1.0f;

        /// <summary>
        /// 画质 0 = 极速 ，1 = 高清 ， 2 = 完美
        /// </summary>
        private int _quality = -1;

        private bool _isFarView = false;

        /// <summary>
        /// 是否开启天气效果 默认为true
        /// </summary>
        private bool _bWeather = true;

        public BaseAppSetting()
        {
            if (ins != null)
            {
                throw new Exception("BaseAppSetting Singleton already constructed!");
            }

            ins = this;

            defaultInit();
        }

        protected virtual void defaultInit()
        {
            _isFarView = getInt("isFarView", 1) == 1;
            _bWeather = getInt("bWeather", 1) == 1;

            _soundEnabled = getInt("soundEnabled", 1) == 1;
            _musicEnabled = getInt("musicEnabled", 1) == 1;

            _soundPercent = getFloat("soundPercent", 1.0f);
            _musicPercent = getFloat("musicPercent", 1.0f);

            _quality = getInt("quality", 2);

            int t = _quality;
            //第一次使用，则判断机器性能进行匹配
            if (SystemInfo.graphicsMultiThreaded == false)
            {
                t = getLittle(t, 0);
            }
            else
            {
                float systemMemorySize = SystemInfo.systemMemorySize;
                if (systemMemorySize >= 2048)
                    t = getLittle(t, 2);
                else if (systemMemorySize <= 1024)
                    t = getLittle(t, 0);
            }

            quality = _quality;
        }

        protected bool HasKey(string key_)
        {
            return PlayerPrefs.HasKey(PREFIX + key_);
        }
        protected int getInt(string key, int def=0)
        {
            return PlayerPrefs.GetInt(PREFIX+key, def);
        }
        protected void setInt(string key, int value=0)
        {
            PlayerPrefs.SetInt(PREFIX+key, value);
        }
        protected float getFloat(string key, float def=0.0f)
        {
            return PlayerPrefs.GetFloat(PREFIX+key, def);
        }
        protected void setFloat(string key, float value=0.0f)
        {
            PlayerPrefs.SetFloat(PREFIX+key, value);
        }

        public int quality
        {
            get
            {
                return _quality;
            }
            set
            {
                if (_quality != value)
                {
                    _quality = value;
                    setInt("quality", value);
                    doQuality(value);
                }
            }
        }

        protected int getLittle(int d1_, int d2_)
        {
            return d1_ > d2_ ? d2_ : d1_;
        }

        protected virtual void doQuality(int level)
        {
            if (level == 0)
                QualitySettings.SetQualityLevel(0);
            else if(level == 1)
                QualitySettings.SetQualityLevel(2);
            else if (level == 2)
                QualitySettings.SetQualityLevel(3);
        }

       
        /// <summary>
        /// 是否是远视角 默认为 true
        /// </summary>
        public bool isFarView
        {
            get{return _isFarView; }
            set
            {
                _isFarView = value;
                setInt("isFarView", value == false ? 0 : 1);
            }
        }

       
        /// <summary>
        /// 是否开启音乐 默认true
        /// </summary>
        public bool musicEnabled
        {
            get { return _musicEnabled; }
            set
            {
                _musicEnabled = value;
                setInt("musicEnabled", value == false ? 0 : 1);
            }
        }

        /// <summary>
        /// 音乐音量百分比 默认100%
        /// </summary>
        public float musicPercent
        {
            get { return _musicPercent; }
            set
            {
                _musicPercent = value;
                setFloat("musicPercent", value); }
        }

        /// <summary>
        /// 是否开启音效 默认true
        /// </summary>
        public bool soundEnabled
        {
            get { return _soundEnabled; }
            set
            {
                _soundEnabled = value;
                setInt("soundEnabled", value ? 1 : 0);
            }
        }

      
        /// <summary>
        /// 音效音量百分比 默认100%
        /// </summary>
        public float soundPercent
        {
            get
            {
                return _soundPercent;
            }
            set
            {
                _soundPercent = value;
                setFloat("soundPercent", value);
            }
        }

      
        public bool BWeather
        {
            get { return _bWeather; }
            set
            {
                _bWeather = value;
                setInt("bWeather", value ? 1 : 0);
            }
        }
    }
}