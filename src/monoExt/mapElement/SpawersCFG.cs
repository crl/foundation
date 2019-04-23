using System;
using System.Collections.Generic;
using foundation;
using UnityEngine;

namespace gameSDK
{
    [AddComponentMenu("Lingyu/SpawersCFG")]
    public class SpawersCFG : MonoCFG
    {
        protected float radius = 0.4f;
        /// <summary>
        /// 区域刷怪列表
        /// </summary>
        public List<SpawersZoneVO> list = new List<SpawersZoneVO>();
        /// <summary>
        /// 额外连接点
        /// </summary>
        public List<KeyVector2> connList = new List<KeyVector2>();

        public string fileName;

        public bool addConn(KeyVector2 v)
        {
            if (v.x == v.y)
            {
                return false;
            }

            foreach (KeyVector2 item in connList)
            {
                if (item.isEqual(v))
                {
                    return false;
                }
            }
            connList.Add(v);
            return true;
        }
        

        public SpawersZoneVO getSpawersZoneCfgByGUID(string guid)
        {
            foreach (SpawersZoneVO item in list)
            {
                if (item.guid == guid)
                {
                    return item;
                }
            }

            return null;
        }

        public SpawersZoneVO getNearSpawersZoneCfg(Vector2 value)
        {
            int nearStartIndex = -1;
            float nearStart = float.MaxValue;
            int len = list.Count;
            for (int i = 0; i < len; i++)
            {
                Rect rect = list[i].rect;
                Vector2 v=new Vector2(rect.x,rect.y);
                float d = Vector2.Distance(value, v);
                if (d < nearStart)
                {
                    nearStart = d;
                    nearStartIndex = i;
                }
            }

            if (nearStart > 200f)
            {
                return null;
            }


            if (nearStartIndex != -1)
            {
                return list[nearStartIndex];
            }
            return null;
        }
    }

    [Serializable]
    public class SpawersZoneVO
    {
        [SerializeField]
        public string guid ="";
        /// <summary>
        /// 区域区间设置
        /// </summary>
        public Rect rect = new Rect();
        /// <summary>
        /// 区域内怪物波数表表
        /// </summary>
        public List<WaveCFG> list = new List<WaveCFG>();

        /// <summary>
        /// 进入区域触发技能文件
        /// </summary>
        public TriggerType enterType = TriggerType.SKILL;
        [PathSelecter(PathSelecterType.SKILL_STOTY)]
        public string enterPath;

        /// <summary>
        /// 出去区域触发技能文件
        /// </summary>
        public TriggerType exitType = TriggerType.SKILL;
        [PathSelecter(PathSelecterType.SKILL_STOTY)]
        public string exitPath;

        private Rect _logicRect = GeomExtension.EMPTY;
        public Rect logicRect
        {
            get
            {
                if (_logicRect == GeomExtension.EMPTY)
                {
                    _logicRect = new Rect(rect.x - rect.width/2, rect.y - rect.height/2, rect.width, rect.height);
                }
                return _logicRect;
            }
        }

        public Vector2 realTimeLogicCenter
        {
            get { return new Vector2(rect.x + rect.width/2, rect.y + rect.height/2); }
        }

        public SpawersZoneVO()
        {
        }

        public string getGUID()
        {
            return guid;
        }
    }

    [Serializable]
    public class WaveCFG
    {
        /// <summary>
        /// 这一波触发条件
        /// </summary>
        public WaveTrigerType trigType=WaveTrigerType.DEFAULT;

        /// <summary>
        /// 刷出这一波时 播放一个技能文件
        /// </summary>
        public TriggerType enterType = TriggerType.SKILL;
        [PathSelecter(PathSelecterType.SKILL_STOTY)]
        public string enterPath;

        [Range(0, 20), Tooltip("当被清理完后,是否重复出现")]
        public int repeat = 0;
        [Range(0.01f, 20f), Tooltip("重复波之间的间隔")]
        public float repeatDelayTime = 0.5f;

        /// <summary>
        /// 这一波将刷出些什么怪
        /// </summary>
        public List<MonsterCFG> list = new List<MonsterCFG>();
    }

    [Serializable]
    public class MonsterCFG
    {
        /// <summary>
        /// 怪物被刷出来的 顶视图 位置(游戏内必须转为3D视图坐标)
        /// </summary>
        public Vector2 position;

        public MonsterCFGType type = MonsterCFGType.AUTO;

        /// <summary>
        /// 怪物面部朝向
        /// </summary>
        [Range(0, 360)] public short euler;
        
        /// <summary>
        /// 怪物读取的配置文件id;
        /// </summary>
        public string id;

        /// <summary>
        /// 怪物出生时播放的技能文件
        /// </summary>
        [PathSelecter(PathSelecterType.SKILL)]
        public string enterAction;
        public string pathName;


        [Range(0.01f, 20f)] public float delayTime = 0.5f;

        /// <summary>
        /// 这只怪被重复刷新的次数
        /// </summary>
        [Range(0, 20)] public int repeat = 0;
        /// <summary>
        /// 这只怪被重复刷新的每次的延迟
        /// </summary>
        [Range(0.01f, 20f)] public float repeatDelayTime = 0.5f;


        private Vector3 _logicPosition = Vector3.zero;
        public Vector3 getLogicPosition(SpawersZoneVO zone)
        {
            _logicPosition.x = position.x + zone.rect.x;
            _logicPosition.z = position.y + zone.rect.y;
            return _logicPosition;
        }
    }
}