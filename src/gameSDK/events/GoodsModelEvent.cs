using foundation;

namespace gameSDK
{
    public class GoodsModelEvent:EventX
    {
        public const string ITEM_ADD = "itemAdd";
        public const string ITEM_COUNT_CHANGE = "itemCountChange";

        public long guid;
        public string id;

        public GoodsModelEvent(string type, object data=null) : base(type, data)
        {
            
        }

        public static string ARRANGEMENT { get; internal set; }
    }
}