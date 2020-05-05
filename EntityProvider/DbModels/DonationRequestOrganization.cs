using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class DonationRequestOrganization
    {
        public int Id { get; set; }
        public int DonationRequestId { get; set; }
        public int OrganizationId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? ModeratorId { get; set; }
        public int? VolunteerId { get; set; }
        public int Status { get; set; }

        public virtual DonationRequest DonationRequest { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
