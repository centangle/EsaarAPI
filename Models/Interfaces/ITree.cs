﻿using System.Collections.Generic;

namespace Models.Interfaces
{
    public interface ITree<T> : IBase
    {
        int? ParentId { get; set; }
        ICollection<T> children { get; set; }
    }
    public interface IPeripheral
    {
        bool IsPeripheral { get; set; }
    }
}
