using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityProvider.Helpers
{
    public class TreeTraversal<T>
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? RootId { get; set; }
        public T Node { get; set; }
    }
}
