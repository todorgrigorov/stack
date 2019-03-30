using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.DependencyInjection;
using AFContainer = Autofac.IContainer;

namespace Stack.Registry.Autofac
{
    public class AutofacContainer : IContainer
    {
        public AutofacContainer()
        {
            builder = new ContainerBuilder();
        }
        public AutofacContainer(IServiceCollection services)
            : this()
        {
            builder.Populate(services);
        }

        public void Configure(ContainerConfigurationOptions options = null)
        {
            if (options != null)
            {
                if (options.ResolveUnknownTypes)
                {
                    builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
                }
            }

            container = builder.Build();
        }

        public IServiceProvider AsServiceProvider()
        {
            Assure.NotNull(container, nameof(container));
            return new AutofacServiceProvider(container);
        }

        public bool IsRegistered(Type declaration)
        {
            Assure.NotNull(declaration, nameof(declaration));
            return container.IsRegistered(declaration);
        }
        public bool IsRegistered<TDeclaration>()
        {
            return IsRegistered(typeof(TDeclaration));
        }
        public void Register(
            Type declaration,
            Type implementation,
            ServiceLifetime lifetime = ServiceLifetime.Instance)
        {
            TryRegister(declaration, implementation, lifetime);
        }
        public void Register<TDeclaration, TImplementation>(
            ServiceLifetime lifetime = ServiceLifetime.Instance)
        where TImplementation : TDeclaration
        {
            TryRegister(typeof(TDeclaration), typeof(TImplementation), lifetime);
        }

        public IServiceScope BuildScope()
        {
            return new AutofacServiceScope(container);
        }

        #region Private members
        private void TryRegister(
            Type declaration,
            Type implementation,
            ServiceLifetime lifetime = ServiceLifetime.Instance)
        {
            Assure.NotNull(declaration, nameof(declaration));
            Assure.NotNull(implementation, nameof(implementation));
            Assure.Implements(implementation, declaration, nameof(implementation));

            if (container != null && IsRegistered(declaration))
            {
                throw new RegistryException($"Implementation {implementation.FullName} for {declaration.FullName} has already been registered.");
            }

            var registry = builder.RegisterType(implementation).As(declaration);

            switch (lifetime)
            {
                case ServiceLifetime.Instance:
                    registry.InstancePerDependency();
                    break;
                case ServiceLifetime.Static:
                    registry.SingleInstance();
                    break;
                case ServiceLifetime.Scope:
                    registry.InstancePerLifetimeScope();
                    break;
            }
        }

        private ContainerBuilder builder;
        private AFContainer container;
        #endregion
    }
}