using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stack
{
    public abstract class AssemblyLoader
    {
        public AssemblyLoader(Assembly entryAssembly)
        {
            Assure.NotNull(entryAssembly, nameof(entryAssembly));
            Entry = entryAssembly;

            InitializeAppName();
        }

        public abstract List<Assembly> Load();

        #region Protected members
        protected string AppName { get; private set; }
        protected Assembly Entry { get; private set; }
        #endregion

        #region Private members
        private void InitializeAppName()
        {
            // extract the application running Stack's name by getting the assembly and splitting its name by "."
            // by convention, the assemblies should start with the app's real name
            AppName = Entry.GetName().Name;
            if (AppName.Contains("."))
            {
                AppName = AppName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
        }
        #endregion
    }
}
