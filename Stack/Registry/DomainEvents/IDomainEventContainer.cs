namespace Stack.Registry.DomainEvents
{
    public interface IDomainEventContainer
    {
        void RegisterAll();
        void Invoke<T>(T data, DomainEventType eventType) where T : class;
    }
}
