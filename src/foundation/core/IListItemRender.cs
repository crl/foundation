using System;

namespace foundation
{
    public interface IListItemRender:IDataRenderer,IEventDispatcher,IDisposable
    {
        bool isSelected
        {
            get; set; }
        int index
        {
            set;
            get;
        }

        void refresh();


        Action<string, IListItemRender,object> itemEventHandle
        {
            get; set; }
       
    }
}
