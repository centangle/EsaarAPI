using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class OrganizationRequest
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int EntityId { get; set; }
        public int EntityType { get; set; }
        public int? ModeratorId { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
