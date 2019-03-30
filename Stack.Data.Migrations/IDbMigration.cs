using System;

namespace Stack.Data.Migrations
{
    public interface IDbMigration
    {
        Version Version { get; }
        void Migrate();
    }
}
