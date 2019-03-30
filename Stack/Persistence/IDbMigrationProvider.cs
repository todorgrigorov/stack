namespace Stack.Persistence
{
    public interface IDbMigrationProvider
    {
        void Upgrade();
    }
}
