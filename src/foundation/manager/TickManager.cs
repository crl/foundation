using System;
using UnityEngine;

namespace foundation
{
    public enum UpdateType
    {
        Update,
        FixedUpdate,
        LateUpdate,
        LaterCameraUpdate,
        DrawGizmos
    }

    public class TickManager
    {
        private static QueueHandle<float> updateQueue;
        private static QueueHandle<float> fixedUpdateeQueue;
        private static QueueHandle<float> laterUpdateQueue;
        private static QueueHandle<float> laterCameraUpdateQueue;
        private static QueueHandle<float> drawGizmosQueue;
#if UNITY_EDITOR
        private static float _preTime = 0f;
#endif
        private static float _time = 0f;
        private static float _deltaTime = 0f;

        static TickManager()
        {
            updateQueue = new QueueHandle<float>();
            fixedUpdateeQueue = new QueueHandle<float>();
            laterUpdateQueue = new QueueHandle<float>();
            drawGizmosQueue = new QueueHandle<float>();
            laterCameraUpdateQueue = new QueueHandle<float>();


#if UNITY_EDITOR
            _preTime = (float)UnityEditor.EditorApplication.timeSinceStartup;
            UnityEditor.EditorApplication.update -= editorUpdate;
            UnityEditor.EditorApplication.update += editorUpdate;
#endif
        }

        public static float realtimeSinceStartup()
        {
            if (Application.isPlaying)
            {
                return Time.realtimeSinceStartup;
            }
#if UNITY_EDITOR
            return (float)UnityEditor.EditorApplication.timeSinceStartup;
#endif
            return Time.realtimeSinceStartup;
        }


#if UNITY_EDITOR
        private static void editorUpdate()
        {
            _time = (float)UnityEditor.EditorApplication.timeSinceStartup;
            _deltaTime = _time - _preTime;
            _preTime = _time;
        }
#endif

        public static float alwayTime
        {
            get
            {
                if (Application.isPlaying)
                {
                    return Time.time;
                }

                return _time;
            }
        }

        public static float alwayDeltaTime
        {
            get
            {
                if (Application.isPlaying)
                {
                    return Time.deltaTime;
                }

                return _deltaTime;
            }
        }

        public static float getSafeDeltaTime(float max = 0.05f)
        {
            float t = Time.deltaTime;
            if (t > max)
            {
                t = max;
            }
            return t;
        }


        public static bool Add(Action<float> handle, UpdateType type = UpdateType.Update)
        {
            if (handle == null)
            {
                return false;
            }

            switch (type)
            {
                case UpdateType.Update:
                    return updateQueue.___addHandle(handle, 0);
                case UpdateType.FixedUpdate:
                    return fixedUpdateeQueue.___addHandle(handle, 0);
                case UpdateType.LateUpdate:
                    return laterUpdateQueue.___addHandle(handle, 0);
                case UpdateType.DrawGizmos:
                    return drawGizmosQueue.___addHandle(handle, 0);
                case UpdateType.LaterCameraUpdate:
                    return laterCameraUpdateQueue.___addHandle(handle, 0);
                default:
                    return updateQueue.___addHandle(handle, 0);

            }


        }

        /// <summary>
        /// 加入并且马上调用一次
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool AddAntTick(Action<float> handle, UpdateType type = UpdateType.Update)
        {
            if (handle == null)
            {
                return false;
            }

            bool b = Add(handle, type);

            handle(0);
            return b;
        }
        public static bool AddAntTick(ITickable ticker, UpdateType type = UpdateType.Update)
        {
            if (ticker == null)
            {
                return false;
            }

            bool b = Add(ticker.tick, type);
            ticker.tick(0);
            return b;
        }

        public static bool Add(ITickable ticker, UpdateType type = UpdateType.Update)
        {
            return Add(ticker.tick, type);
        }

        public static bool Remove(Action<float> handle)
        {
            bool b = updateQueue.___removeHandle(handle);
            if (b == false)
            {
                b = fixedUpdateeQueue.___removeHandle(handle);
                if (b == false)
                {
                    b = laterUpdateQueue.___removeHandle(handle);
                    if (b == false)
                    {
                        b = laterCameraUpdateQueue.___removeHandle(handle);
                        if (b == false)
                        {
                            b = drawGizmosQueue.___removeHandle(handle);
                        }
                    }
                }
            }
            return b;
        }

        public static bool Remove(ITickable ticker)
        {
            return Remove(ticker.tick);
        }

        public static void Update(float deltaTime)
        {
            updateQueue.dispatch(deltaTime);
        }

        public static void FixedUpdate(float deltaTime)
        {
            fixedUpdateeQueue.dispatch(deltaTime);
        }

        public static void LateUpdate(float deltaTime)
        {
            laterUpdateQueue.dispatch(deltaTime);
        }
        public static void LateCameraUpdate()
        {
            laterCameraUpdateQueue.dispatch(Time.deltaTime);
        }

        public static void OnDrawGizmos(float deltaTime)
        {
            drawGizmosQueue.dispatch(deltaTime);
        }
    }
}

