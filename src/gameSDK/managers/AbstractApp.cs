using gameSDK;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace foundation
{
    public abstract class AbstractApp:FoundationBehaviour
    {
        public const float NAV_NOFOUND = -10000;
        public static float DEF_TIME_SCALE = 1.0f;

        public static int DefaultTargetFrameRate = 30;
        public static bool IsGamePause = false;
        public static bool ForceCheckPointerOverUI = true;

        /// <summary>
        /// 是否含有编辑器代码(用于是否为正式的代码版本)
        /// </summary>
        public static bool IsHasEditorCode
        {
            get;
            protected set;
        }

        public static CoreLoaderQueue coreLoaderQueue { get; protected set; }
        public static BaseSoundsManager soundsManager { get; protected set; }
        public static InputManager inputManager { get; protected set; }

        public static BaseRectTriggerManager rectTriggerManager { get; protected set; }

        /// <summary>
        /// 上一帧的渲染结果图
        /// </summary>
        public static RenderTexture LastFrameRenderTexture;

        public static Canvas UICanvas { get; protected set; }

        public static WeatherType CurrentWeatherType = WeatherType.Fine;
        public static CanvasScaler CanvasScaler { get; protected set; }
        public static GraphicRaycaster GraphicRaycaster { get; protected set; }

        public static Camera UICamera { get; protected set; }
        public static GameObject UI3DContainer { get; protected set; }
        public static GameObject EffectContainer { get; protected set; }
        public static GameObject ActorContainer { get; protected set; }
        public static GameObject PoolContainer { get; protected set; }
        public static GameObject SoundContainer { get; protected set; }
        public static RectTransform UICanvasTransform
        {
            get;
            protected set;
        }

        public static BaseAppSetting appSetting
        {
            get { return BaseAppSetting.GetInstance(); }
        }


        protected static Camera _DefaultMainCamera;
        protected static Camera _MainCamera;
        protected static Light _DefaultSunLight;
        protected static Light _SunLight;

        public static Camera MainCamera
        {
            get
            {
                if (_MainCamera == null)
                {
                    Camera c = Camera.main;
                    if (c)
                    {
                        return c;
                    }
                }

                return _MainCamera;
            }
            set { _MainCamera = value; }
        }

        public static Light SunLight
        {
            get
            {
                if (_SunLight == null)
                {
                    return _DefaultSunLight;
                }
                return _SunLight;
            }
            set { _SunLight = value; }
        }

        protected virtual void autoCreateChildren()
        {
            ActorContainer = GameObject.Find("ActorContainer");
            if (ActorContainer == null)
            {
                EffectContainer = new GameObject("EffectContainer");
                ActorContainer = new GameObject("ActorContainer");
                PoolContainer = new GameObject("PoolContainer");
                SoundContainer = new GameObject("SoundContainer");
            }

            UI3DContainer = GameObject.Find("UI3DContainer");
            if (UI3DContainer == null)
            {
                UI3DContainer = new GameObject("UI3DContainer");
            }
        }

        public AbstractApp()
        {
#if UNITY_EDITOR
            IsHasEditorCode = true;
#else
            IsHasEditorCode=false;
#endif
        }


        private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>();
        public static bool IsPointerOverUI(Vector3 screenPosition, int fingerId = -1)
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                return false;
            }

            if (eventSystem.currentSelectedGameObject != null)
            {
                return true;
            }

            if (eventSystem.IsPointerOverGameObject(fingerId))
            {
                return true;
            }

            if (ForceCheckPointerOverUI)
            {
                var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = screenPosition;
                tempRaycastResults.Clear();
                //GraphicRaycaster uiRaycaster = UICanvas.GetComponent<GraphicRaycaster>();
                eventSystem.RaycastAll(eventDataCurrentPosition, tempRaycastResults);
                return tempRaycastResults.Count > 0;
            }
            return false;
        }

        protected static IReplacBehaviourManager replacBehaviourManager;
        public static void ReplacBehaviour(ICanbeReplacBehaviour value)
        {
            if (replacBehaviourManager != null)
            {
                replacBehaviourManager.replacBehaviour(value);
            }
        }
    }
}