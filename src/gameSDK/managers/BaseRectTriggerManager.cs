using System;
using System.Collections.Generic;
using foundation;
using UnityEngine;

namespace gameSDK
{
    public class BaseRectTriggerManager : FoundationBehaviour
    {
        protected List<PlayMakerTrigger> _rectList = new List<PlayMakerTrigger>();
        protected AbstractBaseObject _hero;

        public virtual void refreashHero(AbstractBaseObject baseObject)
        {
            if (this._hero != null)
            {
                _hero.removeEventListener(ActorMoveEventX.NEXT_STEP, stepHandle);
                _hero.removeEventListener(ActorMoveEventX.REACHED, stepHandle);
                //_hero.removeEventListener(ActorMoveEventX.ANIMATOR_MOVE, stepHandle);
                //_hero.removeEventListener(ActorMoveEventX.TICK, stepHandle);
            }
            this._hero = baseObject;
            if (this._hero != null)
            {
                _hero.addEventListener(ActorMoveEventX.NEXT_STEP, stepHandle);
                _hero.addEventListener(ActorMoveEventX.REACHED, stepHandle);
                //_hero.addEventListener(ActorMoveEventX.ANIMATOR_MOVE, stepHandle);
                //_hero.addEventListener(ActorMoveEventX.TICK, stepHandle);
            }
        }
        public List<PlayMakerTrigger> getRectList()
        {
            return _rectList;
        }

        public virtual bool add(PlayMakerTrigger value)
        {
            if (_rectList.Contains(value))
            {
                return false;
            }
            value.gameObject.SetLayerRecursively(LayerX.IgnoreRaycast);
            _rectList.Add(value);
            return true;
        }

        public virtual void forceCheck()
        {
            if (_hero != null)
            {
                stepHandle(null);
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public virtual void clear(bool triggerZoom = true)
        {
            DebugX.Log("BaseRectTriggerManager clear");

            foreach (var trigger in _currentInRects)
            {
                if (triggerZoom)
                {
                    onTrigger(trigger, false);
                }
            }
            _currentInRects.Clear();

            _rectList.Clear();
        }

        protected override void onDestroy()
        {
            base.onDestroy();
            if (this._hero != null)
            {
                _hero.removeEventListener(ActorMoveEventX.NEXT_STEP, stepHandle);
                _hero.removeEventListener(ActorMoveEventX.REACHED, stepHandle);
                _hero = null;
            }
        }

        private Vector2 pos2D;
        private List<PlayMakerTrigger> _currentInRects = new List<PlayMakerTrigger>();

        protected virtual void stepHandle(EventX e)
        {
            Vector3 position = getHeroRealPosition();

            pos2D.x = position.x;
            pos2D.y = position.z;

            foreach (PlayMakerTrigger rectCfg in _rectList)
            {
                if (rectCfg.Contains(pos2D))
                {
                    if (_currentInRects.Contains(rectCfg) == false)
                    {
                        onTrigger(rectCfg, true);
                        _currentInRects.Add(rectCfg);
                    }
                }
                else
                {
                    if (_currentInRects.Contains(rectCfg))
                    {
                        onTrigger(rectCfg, false);
                        _currentInRects.Remove(rectCfg);
                    }
                }
            }
        }

        protected virtual Vector3 getHeroRealPosition()
        {
            if (_hero != null)
            {
                return _hero.transform.position;
            }
            return Vector3.zero;
        }

        protected virtual void onTrigger(PlayMakerTrigger trigger, bool isIn = true)
        {
            if (_hero != null)
            {
                if (isIn)
                {
                    _hero.simpleDispatch(AreaEvent.AREA_ENTER, trigger);
                }
                else
                {
                    _hero.simpleDispatch(AreaEvent.AREA_EXIT, trigger);
                }
            }
        }
    }
}
