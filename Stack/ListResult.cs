using System.Collections.Generic;

namespace Stack
{
    public class ListResult<T>
    {
        public ListResult(int total, IEnumerable<T> data)
        {
            Total = total;
            Data = data;
        }

        public int Total { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}