using System;

namespace Stack.Data.Migrations
{
    public static class VersionExtensions
    {
        public static Version ToInitial(this Version version)
        {
            version = new Version(1, 0, 0, 0);
            return version;
        }
        public static bool IsInitial(this Version version)
        {
            return version == new Version(1, 0, 0, 0);
        }
    }
}
