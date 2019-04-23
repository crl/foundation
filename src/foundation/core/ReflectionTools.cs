using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace foundation
{

    ///A collection of helper tools relevant to runtime
    public static class ReflectionTools {

		#if !NETFX_CORE
		private const BindingFlags flagsEverything = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		#endif

		///Assemblies
		private static List<Assembly> _loadedAssemblies;
		private static List<Assembly> loadedAssemblies{
        	get
        	{
        		if (_loadedAssemblies == null){
	        		_loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
	        	}

	        	return _loadedAssemblies;
        	}
        }


		private static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();
		public static Type GetType(string typeName){

			Type type = null;

			if (typeMap.TryGetValue(typeName, out type)){
				return type;
			}

			type = Type.GetType(typeName);
			if (type != null){
				return typeMap[typeName] = type;
			}

            foreach (var asm in loadedAssemblies) {
                try { type = asm.GetType(typeName); }
                catch { continue; }
                if (type != null) {
                    return typeMap[typeName] = type;
                }
            }

            //worst case scenario
            foreach(var t in GetAllTypes()){
            	if (t.Name == typeName){
            		return typeMap[typeName] = t;
            	}
            }

			UnityEngine.Debug.LogError(string.Format("Requested Type with name '{0}', could not be loaded", typeName));
            return null;
		}

		///Get every single type in loaded assemblies
		public static Type[] GetAllTypes(){
			var result = new List<Type>();
			foreach (var asm in loadedAssemblies){
				try {result.AddRange(asm.RTGetExportedTypes());}
				catch { continue; }
			}
			return result.ToArray();
		}

		///Get a list of types deriving provided base type
		public static Type[] GetDerivedTypesOf(Type baseType){
			var result = new List<Type>();
			foreach(var asm in loadedAssemblies){
				try {result.AddRange(asm.RTGetExportedTypes().Where(t => t.RTIsSubclassOf(baseType) && !t.RTIsAbstract() ));}
				catch { continue; }
			}
			return result.ToArray();
		}

		private static Type[] RTGetExportedTypes(this Assembly asm){
			return asm.GetExportedTypes();
		}

		//Just a more friendly name for certain (few) types.
		public static string FriendlyName(this Type type){
			if (type == null){ return "NULL"; }
			if (type == typeof(float)){ return "Float"; }
			if (type == typeof(int)){ return "Integer"; }
			return type.Name;
		}

        //Is property static?
        public static bool RTIsStatic(this PropertyInfo propertyInfo){
            return ((propertyInfo.CanRead && propertyInfo.RTGetGetMethod().IsStatic) || (propertyInfo.CanWrite && propertyInfo.RTGetSetMethod().IsStatic));
        }

		public static bool RTIsAbstract(this Type type){
			return type.IsAbstract;
		}

		public static bool RTIsSubclassOf(this Type type, Type other){
			return type.IsSubclassOf(other);
		}

		public static bool RTIsAssignableFrom(this Type type, Type second){
			return type.IsAssignableFrom(second);
		}

		public static FieldInfo RTGetField(this Type type, string name){
			return type.GetField(name, flagsEverything);
		}

		public static PropertyInfo RTGetProperty(this Type type, string name){
			return type.GetProperty(name, flagsEverything);
		}

		public static MethodInfo RTGetMethod(this Type type, string name){
			return type.GetMethod(name, flagsEverything);
		}

		public static FieldInfo[] RTGetFields(this Type type){
			return type.GetFields(flagsEverything);
		}

		public static PropertyInfo[] RTGetProperties(this Type type){
			return type.GetProperties(flagsEverything);
		}

		public static MemberInfo[] RTGetPropsAndFields(this Type type){
			var result = new List<MemberInfo>();
			result.AddRange(type.RTGetFields());
			result.AddRange(type.RTGetProperties());
			return result.ToArray();
		}

		public static MethodInfo RTGetGetMethod(this PropertyInfo prop){
			return prop.GetGetMethod();
		}

		public static MethodInfo RTGetSetMethod(this PropertyInfo prop){
			return prop.GetSetMethod();
		}

		public static Type RTReflectedType(this Type type){
			return type.ReflectedType;
		}

		public static Type RTReflectedType(this MemberInfo member){
			return member.ReflectedType;
		}

		public static T RTGetAttribute<T>(this Type type, bool inherited) where T : Attribute {
			return (T)type.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
		}

		public static T RTGetAttribute<T>(this MemberInfo member, bool inherited) where T : Attribute{
			return (T)member.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
		}

		///Creates a delegate out of Method for target instance
		public static T RTCreateDelegate<T>(this MethodInfo method, object instance){
			return (T)(object)Delegate.CreateDelegate(typeof(T), instance, method);
		}

	}
}