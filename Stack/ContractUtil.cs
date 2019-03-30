using System;

namespace Stack
{
    public static class ContractUtil
    {
        public static bool IsModel(Type type)
        {
            Assure.NotNull(type, nameof(type));
            return type.Inherits(typeof(Model));
        }
        public static bool IsFilter(Type type)
        {
            Assure.NotNull(type, nameof(type));
            return type.Inherits(typeof(Filter));
        }
    }
}
