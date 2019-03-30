namespace Stack.Registry.DomainEvents
{
    public interface IEntityDeleted<T>
    {
        void Deleted(T entity);
    }
}
