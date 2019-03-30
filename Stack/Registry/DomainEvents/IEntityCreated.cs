namespace Stack.Registry.DomainEvents
{
    public interface IEntityCreated<T>
    {
        void Created(T entity);
    }
}
