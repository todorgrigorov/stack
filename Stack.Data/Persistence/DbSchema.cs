using Stack.Configuration;
using Stack.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stack.Data.Persistence
{
    public sealed class DbSchema : IEnumerable<ITableInfo>
    {
        public DbSchema()
        {
            Update();
        }

        public void Update()
        {
            tables = new HashSet<ITableInfo>(Database.Persister.Schema);
        }
        public bool TableExists(string name)
        {
            return tables.Any(t => t.Name == name);
        }
        public bool ColumnExists(string table, string column)
        {
            return tables.Any(t => t.Columns.Any(c => c == column));
        }

        public IEnumerator<ITableInfo> GetEnumerator()
        {
            return tables.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return tables.GetEnumerator();
        }

        #region Private members
        private HashSet<ITableInfo> tables;
        #endregion
    }
}
