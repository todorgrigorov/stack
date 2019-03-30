using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stack
{
    public static class ReflectionExtensions
    {
        #region Field members
        public static FieldInfo LoadField(this Type type, string name)
        {
            return type.GetField(name);
        }
        public static IList<FieldInfo> LoadFields(this Type type, params string[] names)
        {
            return type
                    .GetFields()
                    .Where(f => ShouldFilterByName(f.Name, names))
                    .ToList();
        }
        public static T Get<T>(this FieldInfo field, object instance)
        {
            return (T)field.GetValue(instance);
        }
        public static void Set<T>(this FieldInfo field, object instance, T value)
        {
            field.SetValue(instance, value);
        }
        #endregion

        #region Property members
        public static PropertyInfo LoadProperty(this Type type, string name)
        {
            return type.GetProperty(name);
        }
        public static bool HasProperty(this Type type, string name)
        {
            return LoadProperty(type, name) != null;
        }
        public static IList<PropertyInfo> LoadProperties(this Type type, params string[] names)
        {
            return type
                    .GetProperties()
                    .Where(p => ShouldFilterByName(p.Name, names))
                    .ToList();
        }
        public static IList<PropertyInfo> LoadPropertiesWithAttributes(this Type type, params Type[] attributes)
        {
            Func<Attribute, bool> containsAttribute = a => attributes
                                                                .ToList()
                                                                .Contains(a.GetType());
            return type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => attributes != null ? p.GetCustomAttributes().Any(containsAttribute) : true)
                    .ToList();
        }
        public static T Get<T>(this PropertyInfo property, object instance)
        {
            return (T)property.GetValue(instance);
        }
        public static void Set<T>(this PropertyInfo property, object instance, T value)
        {
            property.SetValue(instance, value);
        }
        public static bool HasValue(this PropertyInfo property, object instance, object value)
        {
            object defaultValue = null;
            if (!property.PropertyType.IsReferenceType())
            {
                defaultValue = property.PropertyType.CreateInstance();
            }

            if (Equals(value, defaultValue))
            {
                return false;
            }
            return true;
        }
        public static bool IsReadable(this PropertyInfo property)
        {
            return property.GetMethod != null;
        }
        public static bool IsWritable(this PropertyInfo property)
        {
            return property.SetMethod != null;
        }
        #endregion

        #region Method members
        public static MethodInfo LoadMethod(this Type type, string name)
        {
            return type.GetMethod(name);
        }
        public static IList<MethodInfo> LoadMethods(this Type type, params string[] names)
        {
            return type
                    .GetMethods()
                    .Where(m => ShouldFilterByName(m.Name, names))
                    .ToList();
        }
        public static T Call<T>(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            return (T)methodInfo.Invoke(instance, parameters);
        }
        #endregion

        #region Attribute members
        public static T LoadAttribute<T>(this Type type)
            where T : Attribute
        {
            return type
                    .LoadAttributes<T>()
                    .FirstOrDefault();
        }
        public static IList<Attribute> LoadAttributes(this Type type, Type attribute)
        {
            return type
                    .GetTypeInfo()
                    .GetCustomAttributes(attribute)
                    .ToList();
        }
        public static IList<T> LoadAttributes<T>(this Type type)
            where T : Attribute
        {
            return type
                    .LoadAttributes(typeof(T))
                    .Cast<T>()
                    .ToList();
        }
        public static bool HasAttribute(this Type type, Type attribute)
        {
            return type
                    .LoadAttributes(attribute)
                    .Any();
        }
        public static T LoadAttribute<T>(this PropertyInfo property)
            where T : Attribute
        {
            return (T)property.GetCustomAttribute(typeof(T));
        }
        public static IList<T> LoadAttributes<T>(this PropertyInfo property)
            where T : Attribute
        {
            return property
                    .LoadAttributes(typeof(T))
                    .Cast<T>()
                    .ToList();
        }
        public static IList<Attribute> LoadAttributes(this PropertyInfo property, Type attribute)
        {
            return property
                    .GetCustomAttributes(true)
                    .Where(a => a.GetType() == attribute)
                    .Cast<Attribute>()
                    .ToList();
        }
        public static bool HasAttribute(this PropertyInfo property, Type attribute)
        {
            return property
                    .LoadAttributes(attribute)
                    .Any();
        }
        #endregion

        #region Type members
        public static Type LoadBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }
        public static Type LoadInterface(this Type type, string name)
        {
            return type.GetTypeInfo().GetInterface(name);
        }
        public static IList<Type> LoadTypes(this Assembly assembly, params string[] names)
        {
            return assembly
                    .GetTypes()
                    .Where(t => ShouldFilterByName(t.Name, names))
                    .ToList();
        }

        public static bool Implements(this Type type, Type interfaceType)
        {
            bool result = true;

            TypeInfo info = interfaceType.GetTypeInfo();
            if (info.IsGenericType)
            {
                result = type
                           .GetInterfaces()
                           .Any(x => x.GetTypeInfo().IsGenericType &&
                                        x.GetGenericTypeDefinition() == interfaceType.GetGenericTypeDefinition());
            }
            else if (type == interfaceType)
            {
                // note: IsAssignableFrom returns true if the types are the same (lol wut?!)
                result = false;
            }
            else
            {
                result = interfaceType.IsAssignableFrom(type);
            }

            return result;
        }
        public static bool Inherits(this Type type, Type baseType)
        {
            return type.GetTypeInfo().IsSubclassOf(baseType);
        }
        public static object CreateInstance(this Type type, params object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }
        public static T CreateInstance<T>(this Type type, params object[] parameters)
        {
            return (T)CreateInstance(type, parameters);
        }
        public static bool IsReferenceType(this Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                if (Nullable.GetUnderlyingType(type) != null)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Private members
        private static bool ShouldFilterByName(string name, string[] names)
        {
            return names != null && names.Length > 0 ? names.Contains(name) : true;
        }
        #endregion
    }
}
