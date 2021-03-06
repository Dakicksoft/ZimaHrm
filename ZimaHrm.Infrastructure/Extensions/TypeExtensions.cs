using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            if (type.IsValueType)
                return false;

            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private static readonly ConcurrentDictionary<Type, object> _defaultValues = new ConcurrentDictionary<Type, object>();
        public static object GetDefaultValue(this Type type)
        {
            return _defaultValues.GetOrAdd(type, t => type.IsValueType ? Activator.CreateInstance(type) : null);
        }

        public static bool IsNumeric(this Type type)
        {
            if (type.IsArray)
                return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }

            return false;
        }

        public static T ToType<T>(this object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Type targetType = typeof(T);
            TypeConverter converter = TypeDescriptor.GetConverter(targetType);
            Type valueType = value.GetType();

            if (targetType.IsAssignableFrom(valueType))
                return (T)value;

            if ((valueType.IsEnum || value is string) && targetType.IsEnum)
            {
                // attempt to match enum by name.
                if (EnumExtensions.TryEnumIsDefined(targetType, value.ToString()))
                {
                    object parsedValue = Enum.Parse(targetType, value.ToString(), false);
                    return (T)parsedValue;
                }

                var message = string.Format("The Enum value of '{0}' is not defined as a valid value for '{1}'.", value, targetType.FullName);
                throw new ArgumentException(message);
            }

            if (valueType.IsNumeric() && targetType.IsEnum)
                return (T)Enum.ToObject(targetType, value);

            if (converter != null && converter.CanConvertFrom(valueType))
            {
                object convertedValue = converter.ConvertFrom(value);
                return (T)convertedValue;
            }

            if (value is IConvertible)
            {
                try
                {
                    object convertedValue = Convert.ChangeType(value, targetType);
                    return (T)convertedValue;
                }
                catch (Exception e)
                {
                    throw new ArgumentException(string.Format("An incompatible value specified.  Target Type: {0} Value Type: {1}", targetType.FullName, value.GetType().FullName), "value", e);
                }
            }

            throw new ArgumentException(string.Format("An incompatible value specified.  Target Type: {0} Value Type: {1}", targetType.FullName, value.GetType().FullName), "value");
        }

        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface))
                            continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties.Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        }

        public static bool IsSubClass(this Type type, Type check)
        {
            Type implementingType;
            return IsSubClass(type, check, out implementingType);
        }

        public static bool IsSubClass(this Type type, Type check, out Type implementingType)
        {
            return IsSubClassInternal(type, type, check, out implementingType);
        }

        private static bool IsSubClassInternal(Type initialType, Type currentType, Type check, out Type implementingType)
        {
            if (currentType == check)
            {
                implementingType = currentType;
                return true;
            }

            // don't get interfaces for an interface unless the initial type is an interface
            if (check.IsInterface && (initialType.IsInterface || currentType == initialType))
            {
                foreach (Type t in currentType.GetInterfaces())
                {
                    if (IsSubClassInternal(initialType, t, check, out implementingType))
                    {
                        // don't return the interface itself, return it's implementor
                        if (check == implementingType)
                            implementingType = currentType;

                        return true;
                    }
                }
            }

            if (currentType.IsGenericType && !currentType.IsGenericTypeDefinition)
            {
                if (IsSubClassInternal(initialType, currentType.GetGenericTypeDefinition(), check, out implementingType))
                {
                    implementingType = currentType;
                    return true;
                }
            }

            if (currentType.BaseType == null)
            {
                implementingType = null;
                return false;
            }

            return IsSubClassInternal(initialType, currentType.BaseType, check, out implementingType);
        }

        [DebuggerStepThrough]
        public static bool HasDefaultConstructor(this Type type)
        {
            if (type.IsValueType)
                return true;

            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Any(ctor => ctor.GetParameters().Length == 0);
        }

        public static IDictionary ToDictionary(this Type type)
        {
            return type?.GetProperties().ToDictionary();
        }
    }
}
