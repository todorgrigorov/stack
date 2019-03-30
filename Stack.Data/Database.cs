using System;
using Stack.Configuration;
using Stack.Data.Persistence;
using Stack.Persistence;
using Stack.Registry;

namespace Stack.Data
{
    public static class Database
    {
        static Database()
        {
            Assure.Configured(StackConfiguration.Current);

            if (!ContainerConfiguration.Current.Container.IsRegistered<IDbPersister>())
            {
                throw new ConfigurationException($"{nameof(IDbPersister)} has not been configured.");
            }
        }

        public static IDbPersister Persister { get; private set; }

        public static DbSchema Schema
        {
            get
            {
                if (tables == null)
                {
                    tables = new DbSchema();
                }
                return tables;
            }
        }

        public static void Connect()
        {
            lock (locker)
            {
                scope = ContainerConfiguration.Current.Container.BuildScope();
                scope.Begin();
                Persister = scope.Get<IDbPersister>();
                Persister.Connect();
            }
        }
        public static void Disconnect()
        {
            lock (locker)
            {
                if (Persister != null)
                {
                    Persister.Disconnect();
                    scope.End();
                }
            }
        }

        public static void InConnection(Action action)
        {
            Assure.NotNull(action, nameof(action));
            try
            {
                Connect();
                action.Invoke();
                Disconnect();
            }
            catch
            {
                if (Persister.TransactionStatus == TransactionStatus.Begun)
                {
                    Persister.RollbackTransaction();
                }

                throw;
            }
            finally
            {
                if (Persister.IsConnected)
                {
                    if (Persister.TransactionStatus != TransactionStatus.NotStarted)
                    {
                        Persister.EndTransaction();
                    }
                    Disconnect();
                }
            }
        }
        public static void InTransaction(Action action)
        {
            Assure.NotNull(action, nameof(action));
            try
            {
                Persister.BeginTransaction();
                action.Invoke();
                Persister.Save();
                Persister.CommitTransaction();
            }
            catch
            {
                if (Persister.TransactionStatus == TransactionStatus.Begun)
                {
                    Persister.RollbackTransaction();
                }

                throw;
            }
            finally
            {
                if (Persister.TransactionStatus != TransactionStatus.NotStarted)
                {
                    Persister.EndTransaction();
                }
            }
        }

        public static void ValidateTable(Type type)
        {
            string name = type.Name.ToCapitalCase();
            if (!Schema.TableExists(name))
            {
                throw new DbObjectException($"The table {name} does not exist. Maybe it has not been included in a migration?");
            }
        }

        #region Private members
        private static readonly object locker = new object();
        private static DbSchema tables;
        private static IServiceScope scope;
        #endregion
    }
}
