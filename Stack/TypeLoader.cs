using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Stack
{
    public static class TypeLoader
    {
        public static void Setup(AssemblyLoader loader)
        {
            Assure.NotNull(loader, nameof(loader));
            assemblies = loader.Load();
        }

        public static Assembly LoadAssembly(string name)
        {
            Assure.NotEmpty(name, nameof(name));
            Assembly assembly = assemblies
                                 .Where(a => a.GetName().Name == name)
                                 .FirstOrDefault();
            if (assembly == null)
            {
                throw new AssemblyLoadingException($"Assembly with the {name} could not be located in the in-memory cache. Maybe it was not loaded correctly or its name was misspelled?");
            }
            return assembly;
        }
        public static IEnumerable<Type> LoadTypes(AssemblyName assemblyName = null, params Type[] typeRestrictions)
        {
            // load all types that implement at least one of the given interface/class types
            // if nothing was passed, return ALL types in ALL assemblies
            IEnumerable<Assembly> filtered = assemblies.Where(GetNameFilter(assemblyName));

            bool restrictOnTypes = (typeRestrictions != null && typeRestrictions.Length != 0);
            foreach (Assembly assembly in filtered)
            {
                IList<Type> types = assembly.LoadTypes();
                foreach (Type type in types)
                {
                    if (IncludeType(type, typeRestrictions, restrictOnTypes))
                    {
                        yield return type;
                    }
                }
            }
        }

        #region Private members
        private static Func<Assembly, bool> GetNameFilter(AssemblyName assemblyName)
        {
            return a => assemblyName != null ? a.GetName().Name.Contains(assemblyName.Name) : true;
        }
        private static bool IncludeType(Type type, Type[] typeRestrictions, bool restrictOnTypes)
        {
            bool include = false;
            if (restrictOnTypes)
            {
                foreach (Type restriction in typeRestrictions)
                {
                    if (type.Implements(restriction) || type.Inherits(restriction))
                    {
                        include = true;
                        break;
                    }
                }
            }
            else
            {
                include = true;
            }
            return include;
        }

        private static List<Assembly> assemblies;
        #endregion
    }
}