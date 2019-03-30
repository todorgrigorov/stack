using System;
using System.Reflection;

namespace Stack.Configuration
{
    public interface IApplication
    {
        string Name { get; }
        Version Version { get; }

        AssemblyName DomainAssembly { get; }
        AssemblyName ContractsAssembly { get; }
        AssemblyName DataAssembly { get; }
        AssemblyName MigrationsAssembly { get; }
        AssemblyName ServicesAssembly { get; }
        AssemblyName WebAssembly { get; }
        AssemblyName WebMvcAssembly { get; }
        AssemblyName XamarinAssembly { get; }
    }
}
