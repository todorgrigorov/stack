namespace Stack.Registry.DomainEvents
{
    public interface IEntityUniqueness<T>
    {
        void Unique(T entity);
    }
}
