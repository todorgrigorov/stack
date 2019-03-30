using Stack.Mapping;
using Stack.Persistence;
using Stack.Registry;
using System;

namespace Stack.Configuration
{
    public class StackConfiguration
    {
        public static StackConfiguration Current
        {
            get
            {
                if (configuration == null)
                {
                    throw new InvalidOperationException($"The core has not yet been configured. Please call {nameof(StackConfiguration)}.{nameof(Setup)} method first.");
                }
                return configuration;
            }
        }

        public static bool IsConfigured
        {
            get
            {
                return configured;
            }
        }
        public static event EventHandler<StackConfigurationEventArgs> Configuring;
        public static event EventHandler<StackConfigurationEventArgs> Configured;

        public static void Setup()
        {
            lock (locker)
            {
                if (!configured)
                {
                    configuration = new StackConfiguration();

                    // notify for the start of the configuration
                    Configuring?.Invoke(null, new StackConfigurationEventArgs(configuration));

                    // validate the configuration
                    ValidateConfig();

                    // upgrade the database
                    if (ContainerConfiguration.Current.Container.IsRegistered<IDbMigrationProvider>())
                    {
                        ContainerConfiguration.Current.Container.BuildScope().Run(s =>
                        {
                            s.Get<IDbMigrationProvider>().Upgrade();
                        });
                    }

                    // the configuration is successful
                    configured = true;

                    // notify for successful configration
                    Configured?.Invoke(null, new StackConfigurationEventArgs(configuration));
                }
            }
        }

        #region Private members
        private static void ValidateConfig()
        {
            if (!ContainerConfiguration.Current.Container.IsRegistered<IApplication>())
            {
                throw new ConfigurationException($"{nameof(IApplication)} has not been configured.");
            }

            if (!ContainerConfiguration.Current.Container.IsRegistered<IMapper>())
            {
                throw new ConfigurationException($"{nameof(IMapper)} has not been configured.");
            }

            bool persisterConfigured = ContainerConfiguration.Current.Container.IsRegistered<IDbPersister>();
            bool migratorConfigirued = ContainerConfiguration.Current.Container.IsRegistered<IDbMigrationProvider>();

            if (!persisterConfigured && migratorConfigirued)
            {
                throw new ConfigurationException($"{nameof(IDbPersister)} has not been configured.");
            }

            if (persisterConfigured)
            {
                if (!migratorConfigirued)
                {
                    throw new ConfigurationException($"{nameof(IDbMigrationProvider)} has not been configured.");
                }

                if (!ContainerConfiguration.Current.Container.IsRegistered<IDbConnectionStringProvider>())
                {
                    throw new ConfigurationException($"{nameof(IDbConnectionStringProvider)} has not been configured.");
                }
            }
        }

        private static readonly object locker = new object();
        private static bool configured;
        private static StackConfiguration configuration;
        #endregion
    }
}
