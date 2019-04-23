using UnityEngine;

namespace foundation
{

    /// <summary>
    /// excel 文件里面的id 选择
    /// </summary>
    public class ExcelIDSelecterAttribute : PropertyAttribute
    {
        public string excelFileID;
        public ExcelIDSelecterAttribute(string excelFileID)
        {
            this.excelFileID = excelFileID;
        }
    }
}