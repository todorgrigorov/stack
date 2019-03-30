using Stack.Persistence;
using System;

namespace Stack.Data.Persistence
{
    public static class DialectExtensions
    {
        public static string ForType(this IDialect dialect, Type type)
        {
            string dbType = string.Empty;
            if (type == typeof(int) || type == typeof(int?))
            {
                dbType = dialect.Integer;
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                dbType = dialect.Decimal;
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                dbType = dialect.Decimal;
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                dbType = dialect.Boolean;
            }
            else if (type == typeof(string))
            {
                dbType = dialect.Text;
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                dbType = dialect.Date;
            }
            else if (type.IsEnum)
            {
                dbType = dialect.Enum;
            }
            return dbType;
        }
    }
}
