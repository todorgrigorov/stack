using System;

namespace Stack.Registry
{
    public class ContainerConfigurationEventArgs : EventArgs
    {
        public ContainerConfigurationEventArgs(ContainerConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ContainerConfiguration Configuration { get; set; }
    }
}
