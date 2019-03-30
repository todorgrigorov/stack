using Stack.Configuration;
using Stack.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stack.Data.Migrations.Persistence
{
    public class DefaultMigrationProvider : IDbMigrationProvider
    {
        public DefaultMigrationProvider(IApplication application)
        {
            this.application = application;
        }

        public void Upgrade()
        {
            lock (locker)
            {
                if (!upgraded)
                {
                    // first, look for all types that implement IDbMigration
                    // instantiate each one and check for version compatibility (if the current db version is lower than the file, i.e. needs upgrade)
                    // put all migrations which have not been run into a set (duplication (different migrations with the same versions) is not allowed)
                    // run them in asending order and upgrade the db accordingly while keeping everything into a transaction
                    // side note: only if the app version is initial (1.0.0.0) also include the initial upgrade file so that all vital objects are properly installed

                    dbVersion = GetDbVersion();
                    appVersion = application.Version;

                    if (dbVersion > appVersion)
                    {
                        throw new DbMigrationException($"Database version {dbVersion.ToString()} is higher than the assembly version {appVersion.ToString()}.");
                    }

                    migrations = new SortedSet<IDbMigration>(new MigrationComparer());
                    IEnumerable<Type> migrationModules = TypeLoader.LoadTypes(
                                                            application.MigrationsAssembly,
                                                            typeof(IDbMigration));
                    foreach (Type module in migrationModules)
                    {
                        IDbMigration instance = (IDbMigration)module.CreateInstance();
                        if ((firstStartup && instance.Version.IsInitial()) || (instance.Version > dbVersion))
                        {
                            if (!migrations.Add(instance))
                            {
                                throw new DbMigrationException($"Multiple migrations found for version {instance.Version}.");
                            }
                        }
                    }

                    if (migrations.Count > 0)
                    {
                        // make sure that the assemblies & the last migration module are declared with the same versions
                        migrationVersion = migrations.Last().Version;
                        if (appVersion < migrationVersion)
                        {
                            throw new DbMigrationException($"Declared assembly version ({appVersion.ToString()}) is lower than the desired migration version ({migrationVersion.ToString()}).");
                        }
                        else if (appVersion > migrationVersion)
                        {
                            throw new DbMigrationException($"Declared assembly version ({appVersion.ToString()}) is higher than the desired migration version ({migrationVersion.ToString()}).");
                        }

                        Run();
                    }

                    upgraded = true;
                }
            }
        }

        #region Private members
        private Version GetDbVersion()
        {
            Version version = null;
            Database.InConnection(() =>
            {
                try
                {
                    // load the most recent version recorded
                    version = Dao
                               .For<VersionInfo>()
                               .LoadFirst<Filter>(sort: new SortOptions[] { new SortOptions(nameof(Entity.Id), SortOrder.Descending) })
                               .ToVersion();
                }
                catch (DbObjectException)
                {
                    // the table VERSION_INFO is missing so most likely the app is starting for the first time
                    firstStartup = true;
                    version = version.ToInitial();
                }
            });
            return version;
        }
        private void SetDbVerion(Version newVersion)
        {
            // record that a version has been incremented
            Dao
             .For<VersionInfo>()
             .Create(new VersionInfo() { Version = newVersion.ToString() });

            dbVersion = newVersion; // also update the in-memory version so it can be compared later
        }
        private void Run()
        {
            Database.InConnection(() =>
            {
                Database.InTransaction(() =>
                {
                    // run every single process into a transaction so that the flow is kept secure
                    Version version = null;
                    try
                    {
                        foreach (IDbMigration migration in migrations)
                        {
                            version = migration.Version;

                            migration.Migrate(); // perform the upgrade
                            TableUtil.UpdateSchema(); // also sync the in-memory schema information
                            SetDbVerion(version); // register a DB upgrade in table VERSION_INFO
                        }

                        ValidateMigrationProcess();
                    }
                    catch (Exception e)
                    {
                        throw new DbMigrationException($"Unable to perform database upgrade to version {version.ToString()}.", e);
                    }
                });
            });
        }
        private void ValidateMigrationProcess()
        {
            // at the end, the db AND the app should have the exact same versions!!
            Version version = migrations.Last().Version;
            if (version != dbVersion)
            {
                throw new DbMigrationException($"Mismatch between versions. Application: {version.ToString()}, DB: {dbVersion.ToString()}.");
            }
        }

        private static readonly object locker = new object();
        private IApplication application;
        private SortedSet<IDbMigration> migrations;
        private bool firstStartup; // indicates that the app is yes to initialize the db
        private Version dbVersion;
        private Version migrationVersion;
        private Version appVersion;
        private bool upgraded;
        #endregion
    }
}
