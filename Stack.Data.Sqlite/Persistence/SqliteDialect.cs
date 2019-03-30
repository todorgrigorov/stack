using Stack.Persistence;

namespace Stack.Data.Sqlite.Persistence
{
    public class SqliteDialect : IDialect
    {
        public string AutoIncrement
        {
            get
            {
                return "INTEGER PRIMARY KEY AUTOINCREMENT";
            }
        }
        public bool UsePrimaryKeyConstraints
        {
            get
            {
                return false;
            }
        }
        public string Integer
        {
            get
            {
                return "INT";
            }
        }
        public string Decimal
        {
            get
            {
                return "NUMERIC";
            }
        }
        public string Boolean
        {
            get
            {
                return "INT";
            }
        }
        public string Text
        {
            get
            {
                return "TEXT";
            }
        }
        public string Date
        {
            get
            {
                return "TEXT";
            }
        }
        public string Enum
        {
            get
            {
                return "INT";
            }
        }
    }
}
