using System;
using UnityEngine;

namespace foundation
{
    public class DragMonoByInput : MonoBehaviour
    {
        public Action<Vector3> beginDragAction;
        public Action<Vector3> onDragAction;
        public Action<Vector3> endDragAction;

        //是否要开始拖动功能栏
        private bool isRightDrag = false;
        //鼠标按下
        private bool isRightDown = false;

        private InputManager input;
        void Awake()
        {
            input = InputManager.getInstance();
        }

        void OnEnable()
        {
           input.addEventListener(MouseEventX.MOUSE_DOWN, MouseEventHandle);
           input.addEventListener(MouseEventX.MOUSE_UP, MouseEventHandle);
           input.addEventListener(MouseEventX.MOUSE_MOVE, MouseEventHandle);
        }

        void OnDisable()
        {
           input.removeEventListener(MouseEventX.MOUSE_DOWN, MouseEventHandle);
           input.removeEventListener(MouseEventX.MOUSE_UP, MouseEventHandle);
           input.removeEventListener(MouseEventX.MOUSE_MOVE, MouseEventHandle);
        }

        private void MouseEventHandle(EventX obj)
        {
            if (gameObject.activeInHierarchy == false) return;

            Vector3 mousePosition = (Vector3)obj.data;
            if (obj.type == MouseEventX.MOUSE_DOWN)
            {
                bool isClickSkin = UIUtils.IsPointerOverUI(mousePosition, gameObject);
                if (isClickSkin)
                {
                    isRightDown = true;
                    if (beginDragAction != null)
                    {
                        beginDragAction(mousePosition);
                    }
                }
                else
                {
                    isRightDown = false;
                }
            }
            else if (obj.type == MouseEventX.MOUSE_MOVE)
            {
                if (isRightDown)
                {
                    isRightDrag = true;
                    if (onDragAction != null)
                    {
                        onDragAction(mousePosition);
                    }

                }
            }
            else if (obj.type == MouseEventX.MOUSE_UP)
            {
                if (isRightDrag == true)
                {
                    isRightDown = false;
                    isRightDrag = false;

                    if (endDragAction != null)
                    {
                        endDragAction(mousePosition);
                    }
                }
            }
        }
    }
}
