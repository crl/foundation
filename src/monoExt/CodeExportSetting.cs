using UnityEngine;

namespace foundation
{
    public class CodeExportSetting : MonoBehaviour
    {
        /// <summary>
        /// 如果有父模块，则代码生成到父模块下
        /// </summary>
        public string parentModuleName = "";

        /// <summary>
        /// 只生成Mediator和view
        /// </summary>
        public bool onlyGenerateMediator = true;
    }
}
