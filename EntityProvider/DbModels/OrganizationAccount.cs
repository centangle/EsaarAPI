using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class OrganizationAccount
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string AccountNo { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
