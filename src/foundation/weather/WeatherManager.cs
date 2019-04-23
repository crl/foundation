using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace foundation
{
    public class WeatherManager : MonoBehaviour
    {
        private static DateTime thisDay = new DateTime(2017, 1, 1);
        private const int secondsOneDay = 24 * 3600;
      
        [Header("Hours")]
        public bool _isFreezeHour = false;
        [Range(0, 24)]
        public float _currentHour = 0;
        public int timeScale = 1;
        public List<DayNightParms> hoursKeys = new List<DayNightParms>();

        [Header("WeatherType")]
        public Light _sunLight;
        public Color _rainColor = Color.gray;
        public WeathTypeParms rainParms=new WeathTypeParms();
        public WeathTypeParms snowParms=new WeathTypeParms();
        public WeathTypeParms fineParms = new WeathTypeParms();
        [Range(0f, 10f)]
        public float changeWeatherTime = 5.0f;
        public WeatherType weatherType = WeatherType.Fine;

        [Header("WeatherAuto")]
        public bool _isAutoWeather = false;
        /// <summary>
        /// 自动切换天气时间(秒)
        /// </summary>
        [Range(20,500)]
        public int autoWeatherSeconds = 50;

        protected int lastWeatherTime = 0;
        protected Color _currentRainColor;

        protected WeatherType _CurrentWeatherType;

        protected float _oldGloss = 0;
        protected Color _oldForgColor = Color.white;
        protected Light rainLight = null;

        protected WeatherType CurrentWeatherType
        {
            get { return _CurrentWeatherType; }
            set
            {
                _CurrentWeatherType = value;
                AbstractApp.CurrentWeatherType = value;
                
                if (value == WeatherType.Rain)
                {
                    if (rainLight == null)
                    {
                        GameObject go = new GameObject("__rainLight");
                        go.transform.SetParent(transform);
                        rainLight = go.AddComponent<Light>();
                        rainLight.color = Color.white;
                        rainLight.type = LightType.Directional;
                        go.transform.localRotation = Quaternion.Euler(85.0f, 0, 0);
                        go.transform.localPosition = Vector3.zero;
                    }
                    rainLight.gameObject.SetActive(true);
                }
                else
                {
                    if (rainLight != null)
                        rainLight.gameObject.SetActive(false);
                }
            }
        }
        protected WeatherType OldWeatherType = WeatherType.Fine;
        private BaseAppSetting appSetting;
        protected virtual void Awake()
        {
            appSetting = BaseAppSetting.GetInstance();
            Shader.DisableKeyword("_SNOW_ON");
            Shader.DisableKeyword("_RAIN_ON");
            Shader.SetGlobalFloat("WeatherConver", 0);
            Shader.SetGlobalColor("WeatherColor", Color.white);
            Shader.SetGlobalFloat("Gloss", 0);
            _currentRainColor = Color.white;

            rainParms.init(this);
            snowParms.init(this);
            fineParms.init(this);

            snowParms.Gloss = 0;

            CurrentWeatherType =WeatherType.Fine;
            OldWeatherType=WeatherType.Fine;
        }

        protected virtual void OnEnable()
        {
            Shader.EnableKeyword("_WEATHER_ON");

            Shader.DisableKeyword("_SNOW_ON");
            Shader.DisableKeyword("_RAIN_ON");
        }

        protected virtual void OnDisable()
        {
            Shader.DisableKeyword("_WEATHER_ON");
            Shader.DisableKeyword("_SNOW_ON");
            Shader.DisableKeyword("_RAIN_ON");
        }

        protected Coroutine changeWeatherCoroutine;
        private WeathTypeParms currentWeathTypeParm;
        protected virtual IEnumerator changeWeather(WeatherType newWeatherType)
        {
            if (currentWeathTypeParm != null)
            {
                currentWeathTypeParm.sleep();
                currentWeathTypeParm = null;
            }

            float startTime = Time.time;
            float deltaTime = Time.time - startTime;
            while (deltaTime <= changeWeatherTime)
            {
                float v = deltaTime / changeWeatherTime;
                Shader.SetGlobalFloat("WeatherConver", 1.0f - v);
                if (OldWeatherType == WeatherType.Rain)
                {
                    _currentRainColor = Color.Lerp(_rainColor, Color.white, v);
                }
                yield return null;
                deltaTime = Time.time - startTime;
            }

            Shader.SetGlobalFloat("WeatherConver", 0);
            Shader.DisableKeyword("_SNOW_ON");
            Shader.DisableKeyword("_RAIN_ON");

            switch (newWeatherType)
            {
                case WeatherType.Rain:
                    Shader.EnableKeyword("_RAIN_ON");
                    currentWeathTypeParm=rainParms;
                    break;
                case WeatherType.Snow:
                    Shader.EnableKeyword("_SNOW_ON");
                    currentWeathTypeParm=snowParms;
                    break;
                case WeatherType.Fine:
                    Shader.DisableKeyword("_SNOW_ON");
                    Shader.DisableKeyword("_RAIN_ON");
                    currentWeathTypeParm= fineParms;
                    break;
            }

            if (currentWeathTypeParm != null)
            {
                currentWeathTypeParm.awake();
            }

            startTime = Time.time;
            deltaTime = Time.time - startTime;
            while (deltaTime <= changeWeatherTime)
            {
                float v = deltaTime / changeWeatherTime;
                _oldGloss = Mathf.Lerp(_oldGloss , currentWeathTypeParm.Gloss, v);
                Shader.SetGlobalFloat("Gloss", _oldGloss);
                Shader.SetGlobalFloat("WeatherConver", v);
                if (newWeatherType == WeatherType.Rain)
                {
                    _currentRainColor = Color.Lerp(Color.white, _rainColor, v);
                }
                _oldForgColor = Color.Lerp(_oldForgColor, currentWeathTypeParm.forgColor, v);
                RenderSettings.fogColor = _oldForgColor;
                RenderSettings.ambientSkyColor = _oldForgColor;
                if (RenderSettings.skybox != null)
                    RenderSettings.skybox.color = _oldForgColor;

                yield return null;
                deltaTime = Time.time - startTime;
            }
            changeWeatherCoroutine = null;
        }

        protected virtual void Update()
        {
            if (appSetting == null)
            {
                return;
            }
            if (appSetting.BWeather == false || appSetting.quality != 2)
            {
                _isAutoWeather = false;
                weatherType = WeatherType.Fine;
            }
            else
            {
                _isAutoWeather = true;
            }
            if (_isAutoWeather)
            {
                long curTime = DateTime.Now.Ticks / 10000000;
                autoWeatherSeconds = 6*60*60;
                string oldTimeStr = PlayerPrefs.GetString("oldWeatherTime", "");
                long oldTime = 0;
                if (string.IsNullOrEmpty(oldTimeStr) == true)
                    oldTime = curTime - autoWeatherSeconds - 10;
                else
                    oldTime = long.Parse(oldTimeStr);
                if (curTime - oldTime > autoWeatherSeconds)
                {
                    weatherType = randomWeatherType();
                    PlayerPrefs.SetString("oldWeatherTime", curTime.ToString());
                }
            }

            if (weatherType != CurrentWeatherType)
            {
                OldWeatherType = CurrentWeatherType;
                CurrentWeatherType = weatherType;
                if (changeWeatherCoroutine != null)
                {
                    StopCoroutine(changeWeatherCoroutine);
                }
                changeWeatherCoroutine = StartCoroutine(changeWeather(CurrentWeatherType));
            }

            if (currentWeathTypeParm != null)
            {
                currentWeathTypeParm.update();
            }

            DateTime now = DateTime.Now;
            float curSeconds = 0;
            if (_isFreezeHour)
            {
                curSeconds = _currentHour * 3600.0f;
            }
            else
            {
                TimeSpan s = now - thisDay;
                curSeconds = (int) s.TotalSeconds % secondsOneDay;
                curSeconds += s.Milliseconds / 1000.0f;
                curSeconds = (int) (timeScale * curSeconds) % secondsOneDay;
                //映射到虚拟时间
                curSeconds = (curSeconds / (float) secondsOneDay) * 24 * 3600;
                _currentHour = (int) (curSeconds / 3600.0f);
            }
            int count = hoursKeys.Count;
            for (int i = 0; i < count; i++)
            {
                DayNightParms p = hoursKeys[i];
                DayNightParms next = hoursKeys[(i + 1) % count];
                float thisKeySecond = p.hour * 3600;
                float nextKeySecond = next.hour * 3600;
                if (nextKeySecond < thisKeySecond)
                {
                    nextKeySecond += 24 * 3600;
                }
                if (curSeconds >= thisKeySecond && curSeconds <= nextKeySecond)
                {
                    float t = (curSeconds - thisKeySecond) / (nextKeySecond - thisKeySecond);
                    lerpDayNight(p, next, t);
                    break;
                }
            }
        }

        private WeatherType randomWeatherType()
        {
            int rnd = UnityEngine.Random.Range(0,3);
            return (WeatherType) rnd;
        }

        /// <summary>
        /// 过度时间
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        private void lerpDayNight(DayNightParms start, DayNightParms end, float t)
        {
            Color mainLightColor = Color.Lerp(start.mainLightColor, end.mainLightColor, t)* _currentRainColor;
            Color specularColor = Color.Lerp(start.specularColor, end.specularColor, t);
            if (_sunLight != null)
            {
                _sunLight.color = mainLightColor;
                _sunLight.intensity = Mathf.Lerp(start.lightPower, end.lightPower, t);
            }
            Shader.SetGlobalColor("WeatherColor", mainLightColor);
            Shader.SetGlobalColor("WeatherSpecularColor", specularColor);

            RenderSettings.ambientEquatorColor = Color.Lerp(start.ambientColor, end.ambientColor, t);
            //RenderSettings.fogColor = Color.Lerp(start.fogColor, end.fogColor, t);
        }



        protected virtual void OnDestroy()
        {
            Shader.SetGlobalColor("WeatherColor", Color.white);
            Shader.SetGlobalColor("WeatherSpecularColor", Color.white);

            if (rainParms != null)
            {
                rainParms.OnDestroy();
                snowParms.OnDestroy();
                fineParms.OnDestroy();

                rainParms = null;
                snowParms = null;
                fineParms = null;
            }
        }
    }
}