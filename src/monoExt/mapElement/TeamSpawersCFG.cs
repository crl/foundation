using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class TeamSpawersCFG : MonoCFG
    {
        public List<TeamCFG> list = new List<TeamCFG>();
        public string fileName;
    }

    [Serializable]
    public class TeamCFG
    {
        public string key = "";
        /// <summary>
        ///  区域,所有怪以区域x,y为中心定位
        /// </summary>
        public Rect rect = new Rect(Vector2.zero, Vector2.one);

        [Range(0, 100)] public float resetTime = 3.0f;

        public List<TeamMonsterCFG> list = new List<TeamMonsterCFG>();
    }

    public enum MonsterCFGType
    {
        NORMAL=0,
        GENIUS=1,
        LEADER=2,
        BOSS=3,
        AUTO=4
    }

    [Serializable]
    public class TeamMonsterCFG
    {
        /// <summary>
        /// 怪物读取的配置文件id;
        /// </summary>
        [ExcelIDSelecter(ExcelFileIDType.Monster)] public string id;

        /// <summary>
        /// 
        /// </summary>
        public MonsterCFGType type = MonsterCFGType.AUTO;
        /// <summary>
        /// 怪物被刷出来的 顶视图 位置(游戏内必须转为3D视图坐标)
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// 怪物面部朝向
        /// </summary>
        [Range(0, 360)] public short euler;

        public string enterAction;

        private Vector3 _logicPosition = Vector3.zero;

        public Vector3 getLogicPosition(TeamCFG team)
        {
            _logicPosition.x = position.x + team.rect.x;
            _logicPosition.y = position.y;
            _logicPosition.z = position.z + team.rect.y;
            return _logicPosition;
        }
    }
}