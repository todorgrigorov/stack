namespace Stack.Data.Registry.DomainEvents
{
    public interface IEntityQuerying<T>
    {
        void Querying(T filter);
    }
}
