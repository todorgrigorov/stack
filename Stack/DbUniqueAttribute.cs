using System;

namespace Stack
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DbUniqueAttribute : Attribute
    { 
    }
}
