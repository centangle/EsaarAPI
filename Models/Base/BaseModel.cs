using Models.Interfaces;
using System;
using System.Runtime.Serialization;

namespace Models.Base
{
    public abstract class BaseModel : IBase
    {
        public int Id { get; set; }
        [IgnoreDataMember]
        public bool IsDeleted { get; set; }
        [IgnoreDataMember]//
        public bool IsActive { get; set; }
        [IgnoreDataMember]
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [IgnoreDataMember]
        public int UpdatedBy { get; set; }
        [IgnoreDataMember]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
