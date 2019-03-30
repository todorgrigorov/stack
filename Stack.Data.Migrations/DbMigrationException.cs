using System;

namespace Stack.Data.Migrations
{
    public class DbMigrationException : Exception
    {
        public DbMigrationException()
        {
        }
        public DbMigrationException(string message) 
            : base(message)
        {
        }
        public DbMigrationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
