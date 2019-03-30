using Microsoft.EntityFrameworkCore;
using Stack.Data.Persistence.EntityFramework;

namespace Stack.Data.SqlServer.Persistence.EntityFramework
{
    public class EntityFrameworkSqlServerPersistenceBuilder : IEntityFrameworkPersistenceBuilder
    {
        public void Build(DbContextOptionsBuilder optionsBuilder, string connection)
        {
            optionsBuilder.UseSqlServer(connection);
        }
    }
}
