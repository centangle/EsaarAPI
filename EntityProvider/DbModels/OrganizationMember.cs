using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class OrganizationMember
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int OrganizationId { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Member Member { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
