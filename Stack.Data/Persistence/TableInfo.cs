using Stack.Persistence;
using System;
using System.Collections.Generic;

namespace Stack.Data.Persistence
{
    public sealed class TableInfo : ITableInfo, IEquatable<TableInfo>
    {
        public TableInfo(string schema, string name)
        {
            Assure.NotEmpty(schema, nameof(schema));
            Assure.NotEmpty(name, nameof(name));
            Schema = schema;
            Name = name;
            Columns = new List<string>();
        }

        public string Schema { get; set; }
        public string Name { get; set; }
        public IList<string> Columns { get; set; }

        public override bool Equals(object other)
        {
            return Equals((TableInfo)other);
        }
        public bool Equals(TableInfo other)
        {
            if (other == null)
            {
                return false;
            }
            else if (string.IsNullOrEmpty(Name))
            {
                return false;
            }
            else if (string.IsNullOrEmpty(other.Name))
            {
                return false;
            }
            else
            {
                return Name == other.Name;
            }
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
