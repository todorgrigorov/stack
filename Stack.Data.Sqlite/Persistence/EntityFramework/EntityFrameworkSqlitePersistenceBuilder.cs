using Microsoft.EntityFrameworkCore;
using Stack.Data.Persistence.EntityFramework;

namespace Stack.Data.Sqlite.Persistence.EntityFramework
{
    public class EntityFrameworkSqlitePersistenceBuilder : IEntityFrameworkPersistenceBuilder
    {
        public void Build(DbContextOptionsBuilder optionsBuilder, string connection)
        {
            optionsBuilder.UseSqlite(connection);
        }
    }
}
