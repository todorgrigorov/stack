using System;

namespace Stack
{
    public interface ITimestamps
    {
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }
}
