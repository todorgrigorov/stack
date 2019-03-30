using System;

namespace Stack.Configuration
{
    public class StackConfigurationEventArgs : EventArgs
    {
        public StackConfigurationEventArgs(StackConfiguration configuration)
        {
            Configuration = configuration;
        }

        public StackConfiguration Configuration { get; set; }
    }
}
