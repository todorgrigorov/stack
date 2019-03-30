using Stack.Configuration;
using Stack.Data.Persistence;
using Stack.Data.Queries;
using Stack.Persistence;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Stack.Data.Migrations.Queries
{
    public class DdlQueryBuilder : QueryBuilder
    {
        public DdlQueryBuilder(Type type)
            : base(type)
        {
        }

        public Query CreateTable()
        {
            StringBuilder query = new StringBuilder();
            query.Append($"CREATE TABLE {Table} (");

            StringBuilder constraints = new StringBuilder();
            foreach (PropertyInfo property in Properties)
            {
                ColumnResult column = CreateColumn(property);
                query.Append($"{column.ToString()},");
                if (column.HasConstraint())
                {
                    constraints.Append($"{column.Constraint},");
                }
            }

            query.TrimComma();
            if (constraints.Length > 0)
            {
                query.Append($",{constraints.TrimComma()}");
            }
            query.Append(")");

            return new Query(query.ToString());
        }

        public Query DropTable()
        {
            return new Query($"DROP TABLE {Table}");
        }

        public Query CreateColumn(string name)
        {
            PropertyInfo property = GetProperty(name);
            ColumnResult column = CreateColumn(property);
            Query result = new Query($"ALTER TABLE {Table} ADD COLUMN {column.ToString()}");
            if (column.HasConstraint())
            {
                result += new Query($"ALTER TABLE {Table} ADD CONSTRAINT {column.Constraint}");
            }
            return result;
        }
        public Query DropColumn(string name)
        {
            PropertyInfo property = GetProperty(name);
            return new Query($"ALTER TABLE {Table} DROP COLUMN {GetColumn(property)}");
        }

        #region Private members
        private ColumnResult CreateColumn(PropertyInfo property)
        {
            IDialect dialect = Database.Persister.Dialect;
            Type propertyType = property.PropertyType;
            ColumnResult column = new ColumnResult(GetColumn(property));
            column.Required = property.HasAttribute(typeof(RequiredAttribute));

            if (EntityUtil.IsPrimaryKey(property))
            {
                column.Type = dialect.AutoIncrement;
                if (dialect.UsePrimaryKeyConstraints)
                {
                    column.Constraint = $"CONSTRAINT PK_{Table} PRIMARY KEY ({IdColumn})";
                }
            }
            else
            {
                if (EntityUtil.IsForeignKey(property))
                {
                    string name = column.Name.Replace("_ID", string.Empty);
                    string relation = property
                                        .LoadAttribute<DbForeignAttribute>()
                                        .Relation
                                        ?.ToUpper();
                    if (string.IsNullOrEmpty(relation))
                    {
                        relation = name;
                    }

                    column.Type = dialect.ForType(column.Required ? typeof(int) : typeof(int?));
                    column.Constraint = $"CONSTRAINT FK_{Table}_{name} FOREIGN KEY ({column.Name}) REFERENCES {relation}({IdColumn})";
                }
                else
                {
                    column.Type = dialect.ForType(propertyType);
                    if (column.Type == dialect.Text)
                    {
                        StringLengthAttribute dbLength = property.LoadAttribute<StringLengthAttribute>();
                        if (dbLength != null)
                        {
                            column.Type += $"({dbLength.MaximumLength.ToString()})";
                        }
                    }

                    if (EntityUtil.IsUnique(property))
                    {
                        column.Constraint = $"CONSTRAINT UK_{Table}_{column.Name} UNIQUE ({column.Name})";
                    }
                }
            }

            return column;
        }
        #endregion
    }
}
