using System;
using System.Collections;
using System.Collections.Generic;

namespace Stack
{
    public static class Assure
    {
        public static void NotNull(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }
        public static void Configured(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
        }
        public static void NotEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(name);
            }
        }
        public static void NotEmpty(ICollection collection, string name)
        {
            if (collection == null || (collection != null && collection.Count == 0))
            {
                throw new ArgumentNullException(name);
            }
        }
        public static void NotEqual<T>(IComparable<T> value, T comparison, string name)
        {
            if (value.CompareTo(comparison) == 0)
            {
                throw new ArgumentOutOfRangeException(name, "Arguments are not equal.");
            }
        }
        public static void IsGreater<T>(IComparable<T> value, T comparison, string name)
        {
            if (value.CompareTo(comparison) < 0)
            {
                throw new ArgumentOutOfRangeException(name, "Argument must be lower than the value.");
            }
        }
        public static void IsArray(object value, string name)
        {
            if (!value.GetType().IsArray)
            {
                throw new ArgumentException("Argument must be an array.", name);
            }
        }
        public static void IsCollection(object value, string name)
        {
            if (!value.GetType().Implements(typeof(IEnumerable<>)))
            {
                throw new ArgumentException("Argument must be a collection.", name);
            }
        }
        public static void Implements(Type type, Type declaration, string name)
        {
            if (!type.Implements(declaration))
            {
                throw new ArgumentException(name, $"Argument must implement interface {type.FullName}.");
            }
        }
    }
}
