using System;
using System.Collections.Generic;

namespace Stack
{
    public class FilterOptions<TEntity, TFilter>
        where TEntity : class
        where TFilter : Filter
    {
        public FilterOptions()
        {
            Criteria = new List<Func<TEntity, bool>>();
            Modifier = new FilterModifier();
        }

        public TFilter Data { get; set; }
        public List<Func<TEntity, bool>> Criteria { get; set; }
        public FilterModifier Modifier { get; set; }
    }
}
