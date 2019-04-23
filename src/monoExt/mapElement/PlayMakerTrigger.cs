using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{

    public class AreaEvent : EventX
    {
        public const string AREA_ENTER = "AreaEnter";
        public const string AREA_STAY = "AreaStay";
        public const string AREA_EXIT= "AreaExit";

        public AreaEvent(string type, object data) : base(type, data)
        {
        }
    }



    public enum TriggerAreaType
    {
        None,
        Safe,
        Drink,
        Story,
        Jump,
        Event,
        Say
    }

    public enum AreaStateType
    {
        NONE,
        EnterArea,
        ExitArea
    }

    public class AreaAttachment : MonoBehaviour
    {
        public string id;
        public AreaStateType showType = AreaStateType.EnterArea;
        public AreaStateType hideType = AreaStateType.ExitArea;
        public GameObject disappearEffect;

        /// <summary>
        /// 存活时间单位秒 0表示永久
        /// </summary>
        public float lifeTime = 0;
        /// <summary>
        /// 可否重复触发
        /// </summary>
        public bool repeat;

        public string guid {
            get { return "areaAttachment_" + id; }
        }
    }


    [RequireComponent(typeof(Collider))]
    public class PlayMakerTrigger : MonoCFG
    {
        protected PlayMakerFSM[] playMakerFSMs;
        public string filterTag = TagX.Player;
        public TriggerAreaType areaType = TriggerAreaType.None;
        public string id="";

        public GameObject reference;
        

        /// <summary>
        /// 附件
        /// </summary>
        [HideInInspector]
        public List<AreaAttachment> attachments; 

        new private Collider collider;

        public void OnEnable()
        {
            ///不被射线检测到
            this.gameObject.layer = LayerX.GetIgnoreRaycastLayer();
            collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        public void Awake()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.playMakerFSMs = base.GetComponents<PlayMakerFSM>();

            if (attachments.Count > 0)
            {
                attachments.Clear();
            }
            attachments = new List<AreaAttachment>();
            var children = GetComponentsInChildren<AreaAttachment>(true);
            if (children.Length > 0)
            {
                attachments.AddRange(children);
                foreach (var obj in attachments)
                {
                    obj.SetActive(false);
                }
            }
        }

        public bool Contains(Vector2 value)
        {
            if (isDisposed)
            {
                return false;
            }
            Bounds rect=collider.bounds;
            return rect.Contains(new Vector3(value.x, rect.center.y, value.y));
        }
        public bool Contains(Vector3 value)
        {
            return collider.bounds.Contains(value);
        }

        public void fireEvent(bool isIn)
        {
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (string.IsNullOrEmpty(filterTag) == false)
            {
                if (other.tag != filterTag)
                {
                    return;
                }
            }
            other.simpleDispatch(AreaEvent.AREA_ENTER, this);

            foreach (PlayMakerFSM trigger in playMakerFSMs)
            {
                if (trigger.Active && trigger.Fsm.HandleTriggerEnter)
                {
                    trigger.Fsm.OnTriggerEnter(other);
                }
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (string.IsNullOrEmpty(filterTag) == false)
            {
                if (other.tag != filterTag)
                {
                    return;
                }
            }
            other.simpleDispatch(AreaEvent.AREA_STAY, this);

            foreach (PlayMakerFSM trigger in playMakerFSMs)
            {
                if (trigger.Active && trigger.Fsm.HandleTriggerEnter)
                {
                    trigger.Fsm.OnTriggerStay(other);
                }
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (string.IsNullOrEmpty(filterTag) == false)
            {
                if (other.tag != filterTag)
                {
                    return;
                }
            }
            other.simpleDispatch(AreaEvent.AREA_EXIT, this);
            foreach (PlayMakerFSM trigger in playMakerFSMs)
            {
                if (trigger.Active && trigger.Fsm.HandleTriggerExit)
                {
                    trigger.Fsm.OnTriggerExit(other);
                }
            }
        }
    }
}