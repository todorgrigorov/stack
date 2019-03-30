using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stack.Data.Queries
{
    public abstract class QueryBuilder
    {
        public QueryBuilder(Type type)
        {
            Type = type;
            Properties = new List<PropertyInfo>(GetProperties(Type));
            ValidateSchema();
            Table = GetTable(type);
        }

        #region Protected members
        protected Type Type { get; private set; }
        protected List<PropertyInfo> Properties { get; private set; }
        protected string Table { get; private set; }
        protected string IdColumn
        {
            get
            {
                return nameof(Entity.Id).ToUpper();
            }
        }

        protected string GetTable(Type type)
        {
            return type.Name.ToCapitalCase();
        }
        protected string GetColumn(PropertyInfo property)
        {
            string result = null;

            PropertyInfo column = Properties
                                    .Where(c => c.MetadataToken == property.MetadataToken && c.Module.Equals(property.Module))
                                    .FirstOrDefault();
            if (column != null)
            {
                result = property.Name.ToCapitalCase();
            }
            return result;
        }
        protected IEnumerable<string> GetAllColumns()
        {
            foreach (PropertyInfo column in Properties)
            {
                yield return GetColumn(column);
            }
        }

        protected PropertyInfo GetProperty(string name)
        {
            return Properties
                    .Where(p => p.Name == name)
                    .FirstOrDefault();
        }
        protected PropertyInfo[] GetProperties(Type type)
        {
            // get the props from the base class first, in order
            List<Type> properties = new List<Type>();
            Type iteratingType = type;
            do
            {
                properties.Insert(0, iteratingType);
                iteratingType = iteratingType.LoadBaseType();
            }
            while (iteratingType != null);

            return type.LoadProperties()
                       .Where(p => p.IsReadable() && p.IsWritable() && !EntityUtil.IsIgnored(p))
                       .OrderBy(x => properties.IndexOf(x.DeclaringType))
                       .ToArray();
        }
        #endregion

        #region Private members
        private void ValidateSchema()
        {
            int primaryKeys = 0;
            foreach (PropertyInfo property in Properties)
            {
                if (EntityUtil.IsPrimaryKey(property))
                {
                    primaryKeys++;
                }
            }

            if (primaryKeys > 1)
            {
                throw new DbAnnotationException($"Multiple primary keys found for type {Type.FullName}");
            }
        }
        #endregion
    }
}
