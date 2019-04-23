using UnityEngine;

namespace foundation
{
    public class SceneCFG: MonoCFG
    {
        public Rect rect = new Rect();
        public Texture preview;
        public Texture cutview;
        /// <summary>
        /// 初始化剧情id;
        /// </summary>
        public string storyID;
    }
}