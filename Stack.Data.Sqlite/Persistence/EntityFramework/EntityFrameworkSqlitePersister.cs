using System.Collections.Generic;
using System.Data;
using Stack.Data.Persistence;
using Stack.Data.Persistence.EntityFramework;
using Stack.Data.Queries;
using Stack.Persistence;

namespace Stack.Data.Sqlite.Persistence.EntityFramework
{
    public class EntityFrameworkSqlitePersister : EntityFrameworkPersister
    {
        public EntityFrameworkSqlitePersister(
            IEntityFrameworkContextBuilder contextBuilder,
            IEntityFrameworkPersistenceBuilder persistenceBuilder,
            IDbConnectionStringProvider connectionProvider,
            IDbMapper dbMapper)
                : base(contextBuilder, persistenceBuilder, connectionProvider, dbMapper)
        {
        }

        public override IDialect Dialect
        {
            get
            {
                return new SqliteDialect();
            }
        }

        #region Protected members
        protected override IEnumerable<TableInfo> GetSchema(IDbConnection connection, IDbTransaction transaction)
        {
            List<TableInfo> result = new List<TableInfo>();

            // get all tables in the schema
            Query query = new Query("SELECT TBL_NAME FROM SQLITE_MASTER WHERE TYPE = 'table'");
            using (IDbCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = query.ToString();
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new TableInfo("dbo", reader.GetString(0)));
                    }
                }
            }

            // get all columns for each table
            foreach (TableInfo info in result)
            {
                query = new Query($"PRAGMA TABLE_INFO('{info.Name}')");
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = query.ToString();
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            info.Columns.Add(reader.GetString(1));
                        }
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
