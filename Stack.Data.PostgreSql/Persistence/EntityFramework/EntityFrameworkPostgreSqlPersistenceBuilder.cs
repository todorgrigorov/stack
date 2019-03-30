using Microsoft.EntityFrameworkCore;
using Stack.Data.Persistence.EntityFramework;

namespace Stack.Data.PostgreSql.Persistence.EntityFramework
{
    public class EntityFrameworkPostgreSqlPersistenceBuilder : IEntityFrameworkPersistenceBuilder
    {
        public void Build(DbContextOptionsBuilder optionsBuilder, string connection)
        {
            optionsBuilder.UseNpgsql(connection);
        }
    }
}
