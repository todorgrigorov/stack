using Microsoft.EntityFrameworkCore;

namespace Stack.Data.Persistence.EntityFramework
{
    public interface IEntityFrameworkPersistenceBuilder
    {
        void Build(DbContextOptionsBuilder optionsBuilder, string connection);
    }
}
