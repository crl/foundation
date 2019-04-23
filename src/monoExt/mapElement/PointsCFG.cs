using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class PointsCFG : MonoCFG
    {
        public string key="";
        /// <summary>
        /// 所有出入口配置
        /// </summary>
        public List<PointVO> list =new List<PointVO>();
    }

    [Serializable]
    public enum PointType
    {
        Npc,
        /// <summary>
        /// 入口点
        /// </summary>
        Entrance,
        /// <summary>
        /// 传送点
        /// </summary>
        Transmit,
        /// <summary>
        /// 出口点
        /// </summary>
        Exit,
        /// <summary>
        /// 采集物
        /// </summary>
        Collection

    }

    [System.Serializable]
    public class PointVO
    {
        /// <summary>
        /// 点位置
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// 点类型(入口或出口)
        /// </summary>
        public PointType type;

        /// <summary>
        /// 面部朝向
        /// </summary>
        [Range(0, 360)] public short euler;

        /// <summary>
        /// 配置点
        /// </summary>
        [ExcelIDSelecter(ExcelFileIDType.Npc)]
        public string id;
    }

}