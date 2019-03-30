namespace Stack.Registry
{
    public interface IServiceScopeFactory
    {
        IServiceScope BuildScope();
    }
}