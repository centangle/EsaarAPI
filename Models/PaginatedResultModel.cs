using System.Collections.Generic;

namespace Models
{
    public class PaginatedResultModel<M>
    {
        public PaginatedResultModel()
        {
            Items = new List<M>();
        }
        public int TotalCount { get; set; }
        public List<M> Items { get; set; }
    }
}
