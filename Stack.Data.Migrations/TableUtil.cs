namespace Stack.Data.Migrations
{
    public static class TableUtil
    {
        public static void AssureNotExisting(string name)
        {
            if (Database.Schema.TableExists(name))
            {
                throw new DbObjectException($"Table {name} already exists. Maybe it has been specified in multiple migrations?");
            }
        }
        public static void AssureExisting(string name)
        {
            if (!Database.Schema.TableExists(name))
            {
                throw new DbObjectException($"Table {name} does not exist. Maybe it has not been included in a migration?");
            }
        }
        public static void AssureColumnsNotExisting(string table, string column)
        {
            if (Database.Schema.ColumnExists(table, column))
            {
                throw new DbObjectException($"Column {column} on table {table} already exists. Maybe it has been specified in multiple migrations?");
            }
        }
        public static void AssureColumnExisting(string table, string column)
        {
            if (!Database.Schema.ColumnExists(table, column))
            {
                throw new DbObjectException($"Column {column} on table {table} does not exist. Maybe it has not been included in a migration?");
            }
        }
        public static void UpdateSchema()
        {
            Database.Schema.Update();
        }
    }
}
