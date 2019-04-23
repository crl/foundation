using System.Reflection;

namespace foundation
{
    public class ReflectionAccessor
    {
        public ReflectionAccessor()
        {
        }

        public static bool TryGetMethod(object instance, string methodName, out MethodInfo v)
        {
            MethodInfo methodInfo = instance.GetType().GetMethod(methodName);
            if (methodInfo != null)
            {
                v = methodInfo;
                return true;
            }
            v = null;
            return false;
        }


        public static bool TryGetValue(object instance, string memberName, out object v)
        {
            PropertyInfo propertyInfo = instance.GetType().GetProperty(memberName);
            if (propertyInfo != null)
            {
                v = propertyInfo.GetValue(instance, null);
                return true;
            }
            v = null;

            return false;
        }

        public static void SetValue(object instance, string memberName, object newValue)
        {
            PropertyInfo propertyInfo = instance.GetType().GetProperty(memberName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(instance, newValue, null);
            }

        }
    }
}

