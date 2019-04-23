using System.Reflection;

namespace foundation
{
    public sealed class ClassDefinition
	{
        private string _className;
        private ClassMember[] _members;
        private bool _externalizable;
        private bool _dynamic;

        internal static ClassMember[] EmptyClassMembers = new ClassMember[0];

        public ClassDefinition(string className, ClassMember[] members, bool externalizable, bool dynamic)
		{
			_className = className;
            _members = members;
			_externalizable = externalizable;
			_dynamic = dynamic;
		}

		public string ClassName{ get{ return _className; } }

		public int MemberCount
        { 
            get
            {
                if (_members == null)
                    return 0;
                return _members.Length; 
            } 
        }

        public ClassMember[] Members { get { return _members; } }

		public bool IsExternalizable{ get{ return _externalizable; } }
   
		public bool IsDynamic{ get{ return _dynamic; } }
   
		public bool IsTypedObject{ get{ return (_className != null && _className != string.Empty); } }
 	}

    public sealed class ClassMember
    {
        string _name;
        BindingFlags _bindingFlags;
        MemberTypes _memberType;

        object[] _customAttributes;

        public ClassMember(string name, BindingFlags bindingFlags, MemberTypes memberType, object[] customAttributes)
        {
            _name = name;
            _bindingFlags = bindingFlags;
            _memberType = memberType;
            _customAttributes = customAttributes;
        }

        public string Name
        {
            get { return _name; }
        }
  
        public BindingFlags BindingFlags
        {
            get { return _bindingFlags; }
        }

        public MemberTypes MemberType
        {
            get { return _memberType; }
        }

        public object[] CustomAttributes
        {
            get { return _customAttributes; }
        }

    }
}
