using System.Collections.Generic;

namespace Stack.Persistence
{
    public interface ITableInfo
    {
        string Schema { get; set; }
        string Name { get; set; }
        IList<string> Columns { get; set; }
    }
}
