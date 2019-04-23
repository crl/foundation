using System.Collections;
using System.Collections.Generic;

namespace foundation
{
    public interface IDataProviderList
    {
        object selectedData { get; set; }
        int selectedIndex { get; set; }

        IListItemRender selectedItem { get; set; }

        List<IListItemRender> childrenList { get; }

        int dataLength { get; }

        IList dataProvider
        {
            get;
            set;
        }
    }
}
