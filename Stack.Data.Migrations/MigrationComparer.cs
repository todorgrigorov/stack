using System.Collections.Generic;

namespace Stack.Data.Migrations
{
    internal class MigrationComparer : IComparer<IDbMigration>
    {
        public int Compare(IDbMigration x, IDbMigration y)
        {
            return x.Version.CompareTo(y.Version);
        }
    }
}
