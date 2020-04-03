using System;

namespace Models.Interfaces
{
    public interface IBase
    {
        int Id { get; set; }
        bool IsDeleted { get; set; }
        bool IsActive { get; set; }
        int CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        int UpdatedBy { get; set; }
        DateTime UpdatedDate { get; set; }
    }
}
