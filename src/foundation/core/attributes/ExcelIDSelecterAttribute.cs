using UnityEngine;

namespace foundation
{
    public class ExcelFileIDType
    {
        /// <summary>
        ///  npc表
        /// </summary>
        public const string Npc = "npc";
        /// <summary>
        /// 怪物表
        /// </summary>
        public const string Monster = "monster";
        /// <summary>
        /// 采集点
        /// </summary>
        public const string Collection = "collection";

        /// <summary>
        /// 地图表
        /// </summary>
        public const string Map = "map";
    }
        /// <summary>
    /// excel 文件里面的id 选择
    /// </summary>
    public class ExcelIDSelecterAttribute:PropertyAttribute
    {
        public string excelFileID;
        public ExcelIDSelecterAttribute(string excelFileID)
        {
            this.excelFileID = excelFileID;
        }
    }
}