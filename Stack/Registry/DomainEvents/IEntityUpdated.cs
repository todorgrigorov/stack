namespace Stack.Registry.DomainEvents
{
    public interface IEntityUpdated<T>
    {
        void Updated(T entity);
    }
}
