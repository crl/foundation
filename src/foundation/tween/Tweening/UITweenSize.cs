using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using clayui;
using UnityEngine;

public class UITweenSize : UITweener
{
    public Vector2 from = Vector3.one;
    public Vector2 to = Vector3.one;

    private RectTransform mTrans;

    public RectTransform cachedTransform
    {
        get
        {
            if (mTrans == null) mTrans = GetComponent<RectTransform>();
            return mTrans;
        }
    }

    public Vector3 value
    {
        get
        {
            if (cachedTransform != null)
            {
                return cachedTransform.sizeDelta;
            }
            return Vector3.zero;
        }
        set
        {
            if (cachedTransform != null)
            {
                cachedTransform.sizeDelta = value;
            }
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = from * (1f - factor) + to * factor;
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    public static UITweenSize Begin(GameObject go, float duration, Vector3 scale)
    {
        UITweenSize comp = UITweener.Begin<UITweenSize>(go, duration);
        comp.from = comp.value;
        comp.to = scale;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue()
    {
        from = value;
    }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue()
    {
        to = value;
    }

    [ContextMenu("Assume value of 'From'")]
    private void SetCurrentValueToStart()
    {
        value = from;
    }

    [ContextMenu("Assume value of 'To'")]
    private void SetCurrentValueToEnd()
    {
        value = to;
    }

    public override void copyFrom(UITweener value)
    {
        base.copyFrom(value);

        UITweenSize other = value as UITweenSize;
        if (other != null)
        {
            this.from = other.from;
            this.to = other.to;
        }
    }
}

