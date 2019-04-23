using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICenterOnChild : MonoBehaviour, IEndDragHandler, IDragHandler
{
    //将子物体拉到中心位置时的速度
    public float centerSpeed = 9f;

    /// <summary>
    /// 居中回调，告知第几个子物体在中心
    /// </summary>
    public Action<int> onCenter;

    /// <summary>
    /// 拖动代理事件，告知是否在拖动
    /// </summary>
    public Action<bool> dragAction;

    private ScrollRect _scrollView;
    private Transform _container;

    private List<float> _childrenPos = new List<float>();
    private float _targetPos;
    private bool _centering = false;

    private void Awake()
    {
        _scrollView = GetComponent<ScrollRect>();
        if (_scrollView == null)
        {
            Debug.LogError("CenterOnChild: No ScrollRect");
            return;
        }
        _scrollView.inertia = false;
        _container = _scrollView.content;

        GridLayoutGroup grid;
        grid = _container.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            Debug.LogError("CenterOnChild: No GridLayoutGroup on the ScrollRect's content");
            return;
        }

        _scrollView.movementType = ScrollRect.MovementType.Unrestricted;
        _childrenPos.Clear();
        //计算第一个子物体位于中心时的位置
        float childPosX = _scrollView.GetComponent<RectTransform>().rect.width*0.5f - grid.cellSize.x*0.5f;
        _childrenPos.Add(childPosX);
        int activeChildCount = GetActiveChildCount(_container);
        //缓存所有子物体位于中心时的位置
        for (int i = 0; i < activeChildCount - 1; i++)
        {
            childPosX -= grid.cellSize.x + grid.spacing.x;
            _childrenPos.Add(childPosX);
        }
        lastPosition = _container.localPosition;
    }

    private int GetActiveChildCount(Transform tran)
    {
        int result = 0;
        for (int i = 0; i < tran.childCount; i++)
        {
            if (tran.GetChild(i).gameObject.activeSelf == true)
            {
                result++;
            }
        }
        return result;
    }

    public void ReCalculateChildPosition()
    {
        Awake();
    }

    /// <summary>
    /// 上桢位置，用以计算速度
    /// </summary>
    private Vector3 lastPosition;

    private Vector3 speed;

    private void Update()
    {
        speed = (_container.localPosition - lastPosition)/Time.deltaTime;
        lastPosition = _container.localPosition;
        if (_centering)
        {
            Vector3 v = _container.localPosition;
            v.x = Mathf.Lerp(_container.localPosition.x, _targetPos, centerSpeed*Time.deltaTime);
            _container.localPosition = v;
            if (Mathf.Abs(_container.localPosition.x - _targetPos) < 0.01f)
            {
                _centering = false;
            }
        }
    }

    public void SetCurrentIndex(int index)
    {
        if (index < _childrenPos.Count && index >=0)
        {
            _targetPos = _childrenPos[index];
            _centering = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragAction != null)
        {
            dragAction(false);
        }
        _centering = true;
        _targetPos = FindClosestPos(_container.localPosition.x);
    }

    

    public void OnDrag(PointerEventData eventData)
    {
        if (dragAction != null)
        {
            dragAction(true);
        }
        _centering = false;
    }

    private float FindClosestPos(float currentPos)
    {
        int childIndex = 0;
        float closest = 0;
        float distance = Mathf.Infinity;

        for (int i = 0; i < _childrenPos.Count; i++)
        {
            float p = _childrenPos[i];
            float d = Mathf.Abs(p - currentPos);
            if (d < distance)
            {
                distance = d;
                closest = p;
                childIndex = i;
            }
        }

        if (speed.x < -10)
        {

        }

        //GameObject centerChild = _container.GetChild(childIndex).gameObject;
        if (onCenter != null)
            onCenter(childIndex);

        return closest;
    }
}

