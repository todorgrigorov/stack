using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Stack
{
    public class BinariesAssemblyLoader : AssemblyLoader
    {
        public BinariesAssemblyLoader(Assembly entryAssembly) 
            : base(entryAssembly)
        {
        }

        public override List<Assembly> Load()
        {
            List<Assembly> assemblyList = new List<Assembly>();

            // load only first-party assemblies, located in the bin directory
            string binariesDir = Path.GetDirectoryName(Entry.Location);
            foreach (string lib in Directory.GetFiles(binariesDir, "*.dll", SearchOption.AllDirectories))
            {
                string libName = Path.GetFileName(lib);
                try
                {
                    if (libName.StartsWith("Stack") || libName.StartsWith(AppName))
                    {
                        assemblyList.Add(Assembly.Load(new AssemblyName(libName)));
                    }
                }
                catch (FileLoadException e)
                {
                    throw new AssemblyLoadingException($"Assembly with the name {libName} was found but could no be loaded in memory.", e);
                }
                catch (BadImageFormatException e)
                {
                    throw new AssemblyLoadingException($"Assembly with the name {libName} was found but it seems to be corrupted.", e);
                }
            }

            return assemblyList;
        }
    }
}
