using System;

namespace Stack.Registry
{
    public interface IContainer : IServiceScopeFactory
    {
        void Configure(ContainerConfigurationOptions options = null);

        IServiceProvider AsServiceProvider();

        bool IsRegistered(Type declaration);
        bool IsRegistered<TDeclaration>();
        void Register(
            Type declaration,
            Type implementation,
            ServiceLifetime lifetime = ServiceLifetime.Instance);
        void Register<TDeclaration, TImplementation>(
            ServiceLifetime lifetime = ServiceLifetime.Instance)
                where TImplementation : TDeclaration;
    }
}
