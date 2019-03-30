using Stack.Persistence;

namespace Stack.Data.SqlServer.Persistence
{
    public class SqlServerDialect : IDialect
    {
        public string AutoIncrement
        {
            get
            {
                return "INT IDENTITY(1,1)";
            }
        }
        public bool UsePrimaryKeyConstraints
        {
            get
            {
                return true;
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
                return "DECIMAL(10, 2)";
            }
        }
        public string Boolean
        {
            get
            {
                return "BIT";
            }
        }
        public string Text
        {
            get
            {
                return "NVARCHAR";
            }
        }
        public string Date
        {
            get
            {
                return "DATETIME2";
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
