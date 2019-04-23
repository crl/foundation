
namespace foundation
{
    public class MiEventX
    {
        internal string mType;
        internal object mData;

        internal IEventDispatcher mTarget;
        public MiEventX(string type, object data = null)
        {
            this.mType = type;
            this.mData = data;
        }

        public IEventDispatcher target
        {
            get
            {
                return mTarget;
            }
        }

        public string type
        {
            get
            {
                return mType;
            }

        }

        public object data
        {
            get
            {
                return mData;
            }

        }

        internal void setTarget(IEventDispatcher value)
        {
            this.mTarget = value;
        }


    }
}
