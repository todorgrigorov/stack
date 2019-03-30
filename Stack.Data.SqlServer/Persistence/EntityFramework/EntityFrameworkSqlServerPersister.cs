using System.Collections.Generic;
using System.Data;
using Stack.Data.Persistence;
using Stack.Data.Persistence.EntityFramework;
using Stack.Data.Queries;
using Stack.Persistence;

namespace Stack.Data.SqlServer.Persistence.EntityFramework
{
    public class EntityFrameworkSqlServerPersister : EntityFrameworkPersister
    {
        public EntityFrameworkSqlServerPersister(
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
                return new SqlServerDialect();
            }
        }

        #region Protected members
        protected override IEnumerable<TableInfo> GetSchema(IDbConnection connection, IDbTransaction transaction)
        {
            List<TableInfo> result = new List<TableInfo>();

            Query query = new Query(@"SELECT t.TABLE_SCHEMA, t.TABLE_NAME, c.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLES t
                                      INNER JOIN INFORMATION_SCHEMA.COLUMNS c
                                      ON t.TABLE_NAME = c.TABLE_NAME");
            using (IDbCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = query.ToString();
                using (IDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    while (reader.Read())
                    {
                        string schema = reader.GetString(0);
                        string table = reader.GetString(1);
                        string column = reader.GetString(2);

                        TableInfo info = new TableInfo(schema, table);
                        TableInfo existing = result.Find(t => t.Name == info.Name);
                        if (existing == null)
                        {
                            info.Columns.Add(column);
                            result.Add(info);
                        }
                        else
                        {
                            existing.Columns.Add(column);
                        }
                    }
                }
            }

            return result;
        }
        #endregion
    }
}