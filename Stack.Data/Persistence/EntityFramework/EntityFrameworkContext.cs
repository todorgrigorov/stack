using Microsoft.EntityFrameworkCore;
using Stack.Persistence;

namespace Stack.Data.Persistence.EntityFramework
{
    internal class EntityFrameworkContext : DbContext
    {
        public EntityFrameworkContext(
            IEntityFrameworkContextBuilder contextBuilder,
            IEntityFrameworkPersistenceBuilder persistenceBuilder,
            IDbConnectionStringProvider connectionProvider)
        {
            this.contextBuilder = contextBuilder;
            this.persistenceBuilder = persistenceBuilder;
            this.connectionProvider = connectionProvider;

            Database.AutoTransactionsEnabled = false;
        }

        public DbSet<T> ForSet<T>()
            where T : Entity
        {
            return Set<T>();
        }

        #region Protected members
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Assure.NotNull(connectionProvider.Connection, nameof(connectionProvider.Connection));

            persistenceBuilder.Build(optionsBuilder, connectionProvider.Connection);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            contextBuilder.Build(modelBuilder);
        }
        #endregion

        #region Private members
        private IEntityFrameworkContextBuilder contextBuilder;
        private IEntityFrameworkPersistenceBuilder persistenceBuilder;
        private IDbConnectionStringProvider connectionProvider;
        #endregion
    }
}
