using System.Collections.Generic;
using System.Data;
using Stack.Data.Persistence;
using Stack.Data.Persistence.EntityFramework;
using Stack.Data.Queries;
using Stack.Persistence;

namespace Stack.Data.PostgreSql.Persistence.EntityFramework
{
    public class EntityFrameworkPostgreSqlPersister : EntityFrameworkPersister
    {
        public EntityFrameworkPostgreSqlPersister(
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
                return new PostgreSqlDialect();
            }
        }

        #region Protected members
        protected override IEnumerable<TableInfo> GetSchema(IDbConnection connection, IDbTransaction transaction)
        {
            List<TableInfo> result = new List<TableInfo>();

            Query query = new Query(@"SELECT ist.TABLE_SCHEMA, ist.TABLE_NAME, isc.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLES ist
                                      INNER JOIN INFORMATION_SCHEMA.COLUMNS isc
                                      ON ist.TABLE_NAME = isc.TABLE_NAME
                                      WHERE ist.TABLE_TYPE = 'BASE TABLE' AND ist.TABLE_SCHEMA NOT IN ('pg_catalog', 'information_schema')");
            using (IDbCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = query.ToString();
                using (IDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    while (reader.Read())
                    {
                        string schema = reader.GetString(0);
                        string table = reader.GetString(1).ToCapitalCase();
                        string column = reader.GetString(2).ToCapitalCase();

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