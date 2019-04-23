namespace foundation
{
    public class PanelEvent:EventX
    {
        /// <summary>
        /// 面板的打开; 
        /// </summary>
        public const string SHOW="panel_show";


        ///
        /// 面板的关闭; 
        /// 	
        public const string HIDE= "panel_hide";
		
		public const string MOTION_SHOW_FINISHED= "motion_show_finished";
		public const string MOTION_HIDE_FINISHED= "motion_hide_finished";
		public PanelEvent(string type):base(type)
        {
        }
    }
}