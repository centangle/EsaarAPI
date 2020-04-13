using System.Collections.Generic;

namespace Models.Interfaces
{
    public interface IOneLevelTree<T> : IBase
    {
        int? ParentId { get; set; }
    }
    public interface ITree<T> : IBase
    {
        int? RootId { get; set; }
        int? ParentId { get; set; }
        ICollection<T> children { get; set; }
    }
    public interface IPeripheral
    {
        bool IsPeripheral { get; set; }
    }
}
