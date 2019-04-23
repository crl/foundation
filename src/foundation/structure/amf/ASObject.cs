using System;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace foundation
{
    [TypeConverter(typeof(ASObjectConverter))]
    public class ASObject : Dictionary<string, object>
    {
        private string _typeName;

        public ASObject()
        {
        }
        public ASObject(string typeName)
        {
            _typeName = typeName;
        }

        public ASObject(IDictionary<string, object> dictionary)
            : base(dictionary)
        {
        }
  
        public ASObject(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }

        public bool IsTypedObject
        {
            get { return !string.IsNullOrEmpty(_typeName); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if( IsTypedObject )
                sb.Append(TypeName);
            sb.Append("{");
            int i = 0;
            foreach (KeyValuePair<string, object> entry in this)
            {
                if (i != 0)
                    sb.Append(", ");
                sb.Append(entry.Key);
                i++;
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

 
    public class ASObjectConverter : TypeConverter
    {

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.IsValueType || destinationType.IsEnum)
            {
                return false;
            }
            return true;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            ASObject aso = value as ASObject;
           
            object instance = Activator.CreateInstance(destinationType);
            if (instance != null)
            {
                foreach (string memberName in aso.Keys)
                {
                    object val = aso[memberName];
                    //MemberInfo mi = ReflectionUtils.GetMember(destinationType, key, MemberTypes.Field | MemberTypes.Property);
                    //if (mi != null)
                    //    ReflectionUtils.SetMemberValue(mi, result, aso[key]);

                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = destinationType.GetProperty(memberName);
                    }
                    catch (AmbiguousMatchException)
                    {
                        //To resolve the ambiguity, include BindingFlags.DeclaredOnly to restrict the search to members that are not inherited.
                        propertyInfo = destinationType.GetProperty(memberName, BindingFlags.DeclaredOnly | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
                    }
                    if (propertyInfo != null)
                    {
                        try
                        {
                            val = Convert.ChangeType(val, propertyInfo.PropertyType);
                            if (propertyInfo.CanWrite && propertyInfo.GetSetMethod() != null)
                            {
                                if (propertyInfo.GetIndexParameters() == null || propertyInfo.GetIndexParameters().Length == 0)
                                {
                                    propertyInfo.SetValue(instance, val, null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                    {
                        FieldInfo fi = destinationType.GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
                        try
                        {
                            if (fi != null)
                            {
                                val = Convert.ChangeType(val, fi.FieldType);
                                fi.SetValue(instance, val);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            return instance;
        }
    }
}
