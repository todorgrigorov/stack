using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stack
{
    public static class EntityUtil
    {
        public static void AssureEntity(Type type)
        {
            if (!IsEntity(type))
            {
                throw new ArgumentException($"Specified instance must be of type {typeof(Entity).FullName}.");
            }
        }
        public static void AssureEntity(object instance)
        {
            AssureEntity(instance.GetType());
        }
        public static bool IsEntity(Type type)
        {
            Assure.NotNull(type, nameof(type));
            return type.Inherits(typeof(Entity));
        }

        public static bool IsPrimaryKey(PropertyInfo property)
        {
            return property.HasAttribute(typeof(DbPrimaryAttribute));
        }
        public static bool IsForeignKey(PropertyInfo property)
        {
            return property.HasAttribute(typeof(DbForeignAttribute));
        }
        public static bool IsVersion(PropertyInfo property)
        {
            return property.Name == nameof(Entity.Updated);
        }
        public static bool IsUnique(PropertyInfo property)
        {
            return property.HasAttribute(typeof(DbUniqueAttribute));
        }
        public static bool IsIgnored(PropertyInfo property)
        {
            return property.HasAttribute(typeof(DbIgnoreAttribute));
        }
        public static bool HasForeignRelations(Type type)
        {
            return GetForeignRelations(type).Any();
        }
        public static IEnumerable<PropertyInfo> GetForeignRelations(Type type)
        {
            return type
                    .LoadProperties()
                    .Where(p => IsForeignKey(p));
        }
    }
}
