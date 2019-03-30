using Stack.Registry.DomainEvents;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stack.Registry
{
    public class ContainerConfiguration
    {
        public static ContainerConfiguration Current
        {
            get
            {
                if (configuration == null)
                {
                    throw new InvalidOperationException($"The container has not yet been configured. Please call {nameof(ContainerConfiguration)}.{nameof(Setup)} method first.");
                }
                return configuration;
            }
        }

        public IContainer Container { get; private set; }
        public IDomainEventContainer DomainEventContainer { get; private set; }

        public static bool IsConfigured
        {
            get
            {
                return configured;
            }
        }
        public static event EventHandler<ContainerConfigurationEventArgs> Configuring;
        public static event EventHandler<ContainerConfigurationEventArgs> Configured;

        public static void Setup(
            IContainer container,
            IDomainEventContainer domainEventContainer,
            ContainerConfigurationOptions options = null)
        {
            lock (locker)
            {
                if (!configured)
                {
                    Assure.NotNull(container, nameof(container));
                    Assure.NotNull(domainEventContainer, nameof(domainEventContainer));

                    configuration = new ContainerConfiguration()
                    {
                        Container = container,
                        DomainEventContainer = domainEventContainer
                    };

                    // notify for the start of the configuration
                    Configuring?.Invoke(null, new ContainerConfigurationEventArgs(configuration));

                    // locate all types that implement IRegistryModule and invoke the Register method
                    IEnumerable<Type> registries = TypeLoader.LoadTypes(null, typeof(IRegistryModule));
                    foreach (Type registry in registries)
                    {
                        MethodInfo registerMethod = registry.LoadMethod(nameof(IRegistryModule.Register));
                        IRegistryModule instance = registry.CreateInstance<IRegistryModule>();
                        registerMethod.Call<object>(instance, container);
                    }

                    // register all domain events
                    domainEventContainer.RegisterAll();

                    // add all registered services to the container and build it
                    container.Configure(options);

                    // the configuration is successful
                    configured = true;

                    // notify for successful configration
                    Configured?.Invoke(null, new ContainerConfigurationEventArgs(configuration));
                }
            }
        }

        #region Private members
        private static readonly object locker = new object();
        private static bool configured;
        private static ContainerConfiguration configuration;
        #endregion
    }
}
