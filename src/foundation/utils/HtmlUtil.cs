namespace foundation
{
    public class HtmlUtil
    {
        public static string renderColor(object value, string color)
        {
            if (value == null)
            {
                return "";
            }
           return renderColor(value.ToString(), color);
        }

        public static string renderColor(string value, string color)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            if (color.IndexOf("#") != 0)
            {
                color = "#" + color;
            }
            return "<color='" + color + "'>" + value.ToString() + "</color>";
        }

        /// <summary>
        /// 根据string 生成 带有超链接事件 标签的HtmlText
        /// </summary>
        /// <param name="linkName"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string renderLink(string linkName,string e)
        {
            return "<a href=[" + e +"]>" + linkName + "</a>";
        }

    }
}