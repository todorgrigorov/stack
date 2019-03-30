namespace Stack.Registry.DomainEvents
{
    public interface IEntityCreating<T>
    {
        void Creating(T entity);
    }
}
