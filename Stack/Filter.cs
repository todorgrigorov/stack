using System;

namespace Stack
{
    public abstract class Filter
    {
        public int[] Ids { get; set; }
        public int[] NotIds { get; set; }

        public DateTime? UpdatedBefore { get; set; }
        public DateTime? UpdatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public DateTime? CreatedAfter { get; set; }
    }
}
