using foundation;
using UnityEngine;


public class MouseEventX : EventX
{
    public const string PRESS = "item_press";
    public const string MOUSE_DOWN = "mouse_down";
    public const string MOUSE_UP = "mouse_up";
    public const string MOUSE_OUT = "mouse_out";
    public const string MOUSE_OVER = "mouse_over";
    public const string MOUSE_LEAVE = "mouse_leave";
    public const string MOUSE_ENTER = "mouse_enter";
    public const string MOUSE_MOVE = "mouse_move";
    public const string MOUSE_DRAG = "mouse_drag";
    public Vector2 stagePosition;



    public Touch touch { get; internal set; }

    public MouseEventX(string type,object data = null) : base(type,data)
    {
    }

    public MouseEventX(string type, Vector2 v) : base(type)
    {
        stagePosition = v;
    }
}

