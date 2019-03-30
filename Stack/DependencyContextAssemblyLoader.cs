using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Stack
{
    public class DependencyContextAssemblyLoader : AssemblyLoader
    {
        public DependencyContextAssemblyLoader(Assembly entryAssembly)
            : base(entryAssembly)
        {
        }

        public override List<Assembly> Load()
        {
            List<Assembly> assemblies = new List<Assembly>();

            // load only first-party assemblies by using the default dependency context
            DependencyContext context = DependencyContext.Default;
            foreach (CompilationLibrary lib in context.CompileLibraries)
            {
                string name = lib.Name;

                try
                {
                    if (name.StartsWith("Stack") || name.StartsWith(AppName))
                    {
                        assemblies.Add(Assembly.Load(new AssemblyName(name)));
                    }
                }
                catch (FileLoadException e)
                {
                    throw new AssemblyLoadingException($"Assembly with the name {name} was found but could no be loaded in memory.", e);
                }
                catch (BadImageFormatException e)
                {
                    throw new AssemblyLoadingException($"Assembly with the name {name} was found but it seems to be corrupted.", e);
                }
            }

            return assemblies;
        }
    }
}
