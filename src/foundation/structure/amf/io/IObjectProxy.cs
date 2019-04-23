namespace foundation
{
    public interface IObjectProxy
    {
        bool GetIsExternalizable(object instance);
        bool GetIsDynamic(object instance);
        ClassDefinition GetClassDefinition(object instance);
        object GetValue(object instance, ClassMember member);
        void SetValue(object instance, ClassMember member, object value);
    }
}
