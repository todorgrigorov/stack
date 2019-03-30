namespace Stack.Registry.DomainEvents
{
    public interface IEntityDeleting<T>
    {
        void Deleting(T entity);
    }
}
