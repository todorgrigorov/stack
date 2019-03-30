using Stack.Persistence;

namespace Stack.Data.PostgreSql.Persistence
{
    public class PostgreSqlDialect : IDialect
    {
        public string AutoIncrement
        {
            get
            {
                return "BIGSERIAL PRIMARY KEY";
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
                return "INTEGER";
            }
        }
        public string Decimal
        {
            get
            {
                return "DECIMAL(10, 2)";
            }
        }
        public string Boolean
        {
            get
            {
                return "BOOLEAN";
            }
        }
        public string Text
        {
            get
            {
                return "CHARACTER";
            }
        }
        public string Date
        {
            get
            {
                return "TIMESTAMP";
            }
        }
        public string Enum
        {
            get
            {
                return "INTEGER";
            }
        }
    }
}