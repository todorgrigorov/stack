using Stack.Configuration;

namespace Stack.Registry
{
    public interface IRegistryModule
    {
        void Register(IContainer container);
    }
}
