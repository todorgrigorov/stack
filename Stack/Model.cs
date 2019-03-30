using System;

namespace Stack
{
    public abstract class Model : IIdentifier, ITimestamps
    {
        public int Id { get; set; }
        public bool IsNew
        {
            get
            {
                return Id == 0;
            }
        }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
