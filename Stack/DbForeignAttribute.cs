using System;

namespace Stack
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DbForeignAttribute : Attribute
    {
        public DbForeignAttribute(string relation = null)
        {
            Relation = relation;
        }

        public string Relation { get; set; }
    }
}
