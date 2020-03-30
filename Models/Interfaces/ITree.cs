using System.Collections.Generic;

namespace Models.Interfaces
{
    public interface ITree<T> : IBase
    {
        int? ParentId { get; set; }
        ICollection<T> Childrens { get; set; }
    }
}
