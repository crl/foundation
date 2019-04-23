using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace foundation
{
    public class InputManager: FoundationBehaviour
    {
        private string[] listenerKeys=new string[0];
       
        private Dictionary<string, Action<KeyCode>> eventDownDictonary=new Dictionary<string, Action<KeyCode>>();
        private Dictionary<string, List<KeyCode>> listenerKeysMapping=new Dictionary<string, List<KeyCode>>();

        private Dictionary<KeyCode, List<Action<KeyCode>>> eventUpDictonary = new Dictionary<KeyCode, List<Action<KeyCode>>>();

        private List<Action<int,Vector3, TouchPhase>> touchEventSet = new List<Action<int, Vector3, TouchPhase>>();

        private static KeyCode isRepeatKey = KeyCode.Exclaim;

        protected virtual void Update()
        {
            if ((Input.anyKey || currentDownKeys.Count > 0) &&
                (eventDownDictonary.Count > 0 || eventUpDictonary.Count > 0))
            {
                //DebugX.Log(Input.anyKey+":"+Input.anyKeyDown);
                updateKey();
            }
            if ((Input.touchCount > 0  || isTouchDown) && touchEventSet.Count>0)
            {
                updateTouchEvent();
            }

            if (Input.anyKey || isMouseDown)
            {
                updateMouseEvent();
                return;
            }
        }


        private GameObject checkPicker(Vector2 pointer)
        {
            Camera mainCamera=Camera.main;
            if (mainCamera == null)
            {
                mainCamera=Camera.current;
            }
            if (mainCamera == null)
            {
                return null;
            }
            Ray ray = mainCamera.ScreenPointToRay(pointer);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            return null;
        }

        private List<KeyCode> currentDownKeys=new List<KeyCode>();
        private List<KeyCode> currentUpKeys=new List<KeyCode>(); 
        private List<KeyCode> tempRemove=new List<KeyCode>(); 
       
        private void updateKey()
        {
            tempRemove.Clear();

            int len = currentDownKeys.Count;
            for (int i = 0; i < len; i++)
            {
                KeyCode code=currentDownKeys[i];
           
                if (Input.GetKey(code)) continue;

                tempRemove.Add(code);
                List<Action<KeyCode>> list = null;
                if (eventUpDictonary.TryGetValue(code, out list) == false) continue;

                int k = list.Count;
                for (int j = 0; j < k; j++)
                {
                    list[j](code);
                }
            }
            len = tempRemove.Count;
            for (int i = 0; i < len; i++)
            {
                currentDownKeys.Remove(tempRemove[i]);
            }

            bool hasNew = false;

            int kl = listenerKeys.Length;
            for (int j = 0; j < kl; j++)
            {
                string keyString = listenerKeys[j];
                List<KeyCode> keys = listenerKeysMapping[keyString];
                bool success = true;
                len = keys.Count;
                bool isRepeat=keys[len - 1]== isRepeatKey;

                //是否持续触发回调;
                if (isRepeat)
                {
                    len = len - 1;
                }

                for (int i = 0; i < len; i++)
                {
                    KeyCode key = keys[i];
                    if (Input.GetKey(key))
                    {
                        if (currentDownKeys.Contains(key) == false)
                        {
                            hasNew = true;
                            currentDownKeys.Add(key);
                        }
                    }
                    success &= currentDownKeys.Contains(key);
                }

                //DebugX.Log(success+":"+hasNew);
                if (success)
                {
                    if (isRepeat || hasNew)
                    {
                        Action<KeyCode> action;
                        if(eventDownDictonary.TryGetValue(keyString,out action))
                        {
                            action(keys[0]);
                        }
                    }
                }
            }

        }

        protected bool isMouseDown = false;
        protected bool isTouchDown = false;
        private Vector3 lastPosition=Vector3.zero;
        protected void updateMouseEvent()
        {
            if (Input.GetMouseButton(0))
            {
                if (isMouseDown == false)
                {
                    lastPosition = Input.mousePosition;
                    isMouseDown = true;
                    HandleTouch(-1, Input.mousePosition, TouchPhase.Began);
                    this.simpleDispatch(MouseEventX.MOUSE_DOWN, lastPosition);
                }
                else if (lastPosition.Equals(Input.mousePosition) == false)
                {
                    lastPosition = Input.mousePosition;
                    HandleTouch(-1, Input.mousePosition, TouchPhase.Moved);
                    this.simpleDispatch(MouseEventX.MOUSE_MOVE, lastPosition);
                }
                else
                {
                    lastPosition = Input.mousePosition;
                    HandleTouch(-1, Input.mousePosition, TouchPhase.Stationary);
                    this.simpleDispatch(MouseEventX.MOUSE_ENTER, lastPosition);
                }
            }
            else
            {
                if (isMouseDown == true)
                {
                    lastPosition = Input.mousePosition;
                    isMouseDown = false;
                    HandleTouch(-1, Input.mousePosition, TouchPhase.Ended);
                    this.simpleDispatch(MouseEventX.MOUSE_UP, lastPosition);
                }
            }
        }

        private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
        {
            int len = touchEventSet.Count;
            if (len == 0)
            {
                return;
            }

            for (int i = 0; i < len; i++)
            {
                touchEventSet[i](touchFingerId, touchPosition, touchPhase);
            }
        }

        protected void updateTouchEvent()
        {
            int len = Input.touchCount;
            for (int i = 0; i < len; i++)
            {
                Touch touch = Input.touches[i];
                HandleTouch(touch.fingerId, touch.position, touch.phase);
            }
        }

        public bool isKeyDown(KeyCode key)
        {
            return currentDownKeys.Contains(key);
        }


        public void registKeyDown(KeyCode key, Action<KeyCode> func, bool isRepeatEnter=false,bool shift = false, bool ctrl = false, bool alt = false)
        {
            List<KeyCode> list = new List<KeyCode>();
            list.Add(key);

            string keyValue = key.ToString();
            if (shift)
            {
                list.Add(KeyCode.LeftShift);
                keyValue += "#";
            }
            if (alt)
            {
                list.Add(KeyCode.LeftAlt);
                keyValue += "$";
            }
            if (ctrl)
            {
                list.Add(KeyCode.LeftControl);
                keyValue += "&";
            }

            if (isRepeatEnter)
            {
                list.Add(isRepeatKey);
                keyValue += "@";
            }


            List<KeyCode> keyList;
            if (listenerKeysMapping.TryGetValue(keyValue, out keyList))
            {
                return;
            }

            listenerKeysMapping[keyValue] = list;
            eventDownDictonary[keyValue] = func;

            listenerKeys = listenerKeysMapping.Keys.ToArray();
        }

        public void registKeyUp(KeyCode key, Action<KeyCode> func)
        {
            List<Action<KeyCode>> handleList;
            if (eventUpDictonary.TryGetValue(key,out handleList)==false)
            {
                handleList=new List<Action<KeyCode>>();
                eventUpDictonary.Add(key, handleList);
            }
            List<KeyCode> keyList;
            string keyCode = key.ToString();
            if (listenerKeysMapping.TryGetValue(keyCode, out keyList)==false)
            {
                keyList = new List<KeyCode>();
                keyList.Add(key);
                listenerKeysMapping.Add(keyCode, keyList);
            }

            if (handleList.IndexOf(func) == -1)
            {
                handleList.Add(func);
            }
        }


        public void unregistKeyDown(KeyCode key, Action<KeyCode> func, bool enter = false, bool shift = false,
            bool ctrl = false, bool alt = false)
        {
            string keyValue = key.ToString();
            if (shift)
            {
                keyValue += "#";
            }
            if (alt)
            {
                keyValue += "$";
            }
            if (ctrl)
            {
                keyValue += "&";
            }

            if (enter)
            {
                keyValue += "@";
            }

            List<KeyCode> keyList;
            if (listenerKeysMapping.TryGetValue(keyValue, out keyList)==false)
            {
                return;
            }
            listenerKeysMapping.Remove(keyValue);
            eventDownDictonary.Remove(keyValue);
            listenerKeys = eventDownDictonary.Keys.ToArray();
        }

        public void unregistKeyUp(KeyCode key, Action<KeyCode> func)
        {
            List<Action<KeyCode>> actions;
            if (eventUpDictonary.TryGetValue(key, out actions))
            {
                if (actions.Contains(func))
                {
                    actions.Remove(func);
                }
            }
        }

        public void addTouchEvent(Action<int, Vector3, TouchPhase> func)
        {
            if(touchEventSet.IndexOf(func) == -1)
            {
                touchEventSet.Add(func);
            }
        }

        public void removeTouchEvent(Action<int, Vector3, TouchPhase> func)
        {
            int index = touchEventSet.IndexOf(func);
            if (index != -1)
            {
                touchEventSet.RemoveAt(index);
            }
        }
        public static InputManager getInstance()
        {
            return AbstractApp.inputManager;
        }
    }
}
