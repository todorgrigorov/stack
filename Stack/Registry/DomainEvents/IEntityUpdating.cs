namespace Stack.Registry.DomainEvents
{
    public interface IEntityUpdating<T>
    {
        void Updating(T entity);
    }
}
